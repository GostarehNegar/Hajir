from fastapi import FastAPI
from lib import bus, MsgContext, BaseAgent, CancellationToken, Subjects
from typing import List, Optional, Dict
import models
import logging
from agent_squad.agents import Agent, AgentOptions, AgentResponse
from agent_squad.types import ConversationMessage, ParticipantRole
from agent_squad.classifiers import OpenAIClassifier, OpenAIClassifierOptions
from agent_squad.orchestrator import AgentSquad
from openai import OpenAI
from agentsmith import smith
from tenacity import retry, stop_after_attempt, wait_exponential
server = FastAPI()
api_key = "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"
url = "https://openrouter.ai/api/v1"
logger = logging.getLogger(__name__)
GPT_OSS = "openai/gpt-oss-120b"

template = """Question: {question}
Answer: Let's think step by step."""


def ToChatMessage(msg: ConversationMessage) -> models.ChatMessage:
    return models.ChatMessage(role='user' if msg.role == ParticipantRole.USER else 'assistant',
                              content=msg.content)


def ToConversationMessage(msg: models.ChatMessage) -> ConversationMessage:
    return ConversationMessage(role=ParticipantRole.USER if msg.role == 'user' else ParticipantRole.ASSISTANT,
                               content=msg.content)


def GetConversationText(res: ConversationMessage) -> str:
    if res is None or res.content is None:
        return ''
    if len(res.content) < 1:
        return ''
    ret = res.content[0]
    if isinstance(ret, dict):
        return ret.get('text')
    return f'{ret}'


def GetResponseText(res: AgentResponse) -> str:
    if res == None or res.output == None:
        return ""
    if isinstance(res.output, ConversationMessage):
        return GetConversationText(res.output)
    if isinstance(res.output, str):
        return res.output
    return f'{res.output}'

    return 'res.output.capitalize'


class SessionInfo:
    def __init__(self, id: str, squad: AgentSquad):
        self.id = id
        self.squad = squad
        pass
    pass


sessions: Dict[str, SessionInfo] = {}


class MyOpenAICalssifier(OpenAIClassifier):
    def __init__(self, options):
        super().__init__(options)
        self.client = OpenAI(base_url=url, api_key=api_key)


class AgentProxy(Agent):
    @retry(
        stop=stop_after_attempt(3),
        wait=wait_exponential(multiplier=1, min=4, max=10),
        # retry=retry_if_exception_type((ConnectionError, TimeoutError))
    )
    async def process_request(
        self,
        input_text: str,
        user_id: str,
        session_id: str,
        chat_history: List[ConversationMessage],
        additional_params: Optional[Dict[str, str]] = None
    ) -> ConversationMessage:
        # Send Request To

        _history = [ToChatMessage(msg)
                    for msg in chat_history]
        msg = models.AgentRequest(
            input_text=input_text,
            user_id=user_id,
            session_id=session_id,
            chat_history=_history,
            additional_params=additional_params
        )

        logger.info(f"Proxy Starts")
        res = await bus.request(Subjects.AI.Agents.agent_request(self.name), msg)
        reply = res.GetPayload(models.AgentResponse)
        logger.info(f"Proxy Ends {reply.text} ")

        return ConversationMessage(
            role=ParticipantRole.ASSISTANT.value,
            content=[{"text": f"{reply.text}"}]
        )


class CaptainSquad():
    def __init__(self):
        self.captain: AgentSquad = None
        pass

    async def run(self, cancel: CancellationToken = CancellationToken()):

        # get agents
        logger.info("START")
        # openai_classifier = MyOpenAICalssifier(OpenAIClassifierOptions(
        #     model_id=GPT_OSS
        # ))
        # self.captain = AgentSquad(classifier=openai_classifier)

        # res = await bus.request(Subjects.AI.Agents.Mamagements.listagents, "")
        # agents: List[models.AgentHearBeat] = res.GetPayload(
        #     models.ListAgentsResponse).Agents
        # logger.info(f"{agents.count()} agents discovered")
        # for agent in agents:
        #     proxy = AgentProxy(AgentOptions(
        #         name=agent.Name,
        #         description=agent.Description))
        #     self.captain.add_agent(proxy)

        await bus.subscribe(Subjects.AI.Agents.agent_request("captain"), self.handle)
        pass

    @retry(
        stop=stop_after_attempt(3),
        wait=wait_exponential(multiplier=1, min=4, max=10),
        # retry=retry_if_exception_type((ConnectionError, TimeoutError))
    )
    async def handle(self, msg: MsgContext):

        payload = msg.GetPayload(models.AgentRequest)
        if not payload.session_id in sessions:
            sessions[payload.session_id] = await self.create_session(payload.session_id)
        logger.info(f"Squad starts processing request")
        resp: AgentResponse = await sessions[payload.session_id].squad.route_request(
            user_input=payload.input_text,
            user_id=payload.user_id,
            session_id=payload.session_id,
            additional_params=payload.additional_params,
            stream_response=False
        )
        # print(resp.output)
        response_text = GetResponseText(resp)
        if 'object is not subscriptable' in response_text:
            print('err')
            raise RuntimeError("Failed") 
        await msg.Reply(models.AgentResponse(
            text=GetResponseText(resp)))
        logger.info(f"Squad successfully processed request.")
        pass

    async def NewConversation(ctx: MsgContext):
        logger.info(
            f' New Conversation: {ctx.msg.subject}'
        )
        pass

    async def create_session(self, id: str) -> SessionInfo:
        openai_classifier = MyOpenAICalssifier(OpenAIClassifierOptions(
            api_key="lll",
            model_id=GPT_OSS
        ))
        squad = AgentSquad(classifier=openai_classifier)

        res = await bus.request(Subjects.AI.Agents.Mamagements.listagents, "")
        agents: List[models.AgentHearBeat] = res.GetPayload(
            models.ListAgentsResponse).Agents
        logger.info(f"{len(agents)} agents discovered")
        for agent in agents:
            proxy = AgentProxy(AgentOptions(
                name=agent.Name,
                description=agent.Description))
            squad.add_agent(proxy)
        return SessionInfo(id, squad)


class App(FastAPI):

    async def run(self):
        cap = CaptainSquad()
        await cap.run()
        await smith.start(bus)

        pass


app = App()
