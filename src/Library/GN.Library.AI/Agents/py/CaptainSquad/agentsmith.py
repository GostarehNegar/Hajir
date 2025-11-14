from lib import bus, BaseAgent
from openai import OpenAI
from typing import List, Optional, Dict
from agent_squad.agents import Agent, AgentOptions, OpenAIAgent, OpenAIAgentOptions
from agent_squad.types import ConversationMessage, ParticipantRole
import models
import asyncio
import logging
api_key = "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"
url = "https://openrouter.ai/api/v1"
GPT_OSS = "openai/gpt-oss-120b"

url = "https://api.deepseek.com"
# "sk-3b8842c4b8de41b48ad350662886e849"
api_key = "sk-fec93ba732c046b38b35263b0a4c004d"
GPT_OSS = "deepseek-chat"

client = OpenAI(base_url=url, api_key=api_key)
openai_agent = OpenAIAgent(OpenAIAgentOptions(
    name='OpenAI Agent',
    description='An agent that uses OpenAI API for responses and knows about Iran',
    api_key=api_key,
    client=client,
    model=GPT_OSS
))

logger = logging.getLogger("smith")


def ToConversationMessage(msg: models.ChatMessage) -> ConversationMessage:
    return ConversationMessage(role=ParticipantRole.USER if msg.role == 'user' else ParticipantRole.ASSISTANT,
                               content=msg.content)


class AgnetSmith(BaseAgent):
    def __init__(self, ):
        super().__init__("Smith",
                         'Generic agent that can generally chat with user about.')
        self._agent: OpenAIAgent = None

    async def get_open_ai_agent(self):
        if self._agent == None:
            params: models.LLMParameters = await self.get_llm_params()
            client = OpenAI(base_url=params.Url, api_key=params.ApiKey)
            self._agent = OpenAIAgent(OpenAIAgentOptions(
                name='OpenAI Agent',
                description='An agent that uses OpenAI API for responses and knows about Iran',
                api_key=api_key,
                client=client,
                model=params.Model
            ))
        return self._agent

    async def reply(self, req: models.AgentRequest):
        logger.info(f"Smith starts.")
        _history = [ToConversationMessage(msg)
                    for msg in req.chat_history]
        agent = await self.get_open_ai_agent()
        res = await agent.process_request(req.input_text,
                                          user_id=req.context.UserId,
                                          session_id=req.context.SessionId,
                                          chat_history=req.chat_history,
                                          additional_params=req.context.Parameters)
        logger.info(f"Smith finishes. {res.content[0]}")
        return models.AgentResponse(text=res.content[0].get('text'))
    
        
    
        logger.info(f"Shima starts.")
        _history = [self.ToMessage(msg)
                    for msg in req.chat_history]
        ex = await self.executor()
        response = await ex.ainvoke({
            "input": req.input_text,
            "chat_history": _history
            })
        
        return models.AgentResponse(text=response['output'])
    
    


smith = AgnetSmith()
