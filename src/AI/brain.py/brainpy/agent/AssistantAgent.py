
# https://www.youtube.com/watch?v=H6bCqqw9xyI&t=647s&ab_channel=JamesBriggs
# https://python.langchain.com/docs/modules/agents/agent_types/chat_conversation_agent
#

from brainpy.agent.agentcontext import AgentContext
from langchain_community.chat_models import ChatOpenAI
from langchain.chains.conversation.memory import ConversationBufferWindowMemory
from langchain.chains import RetrievalQA
from langchain.agents import Tool
from langchain.agents import AgentType
from langchain.agents import initialize_agent
from langchain.agents.conversational_chat.prompt import PREFIX, SUFFIX, FORMAT_INSTRUCTIONS
from langchain.agents.conversational_chat.output_parser import ConvoOutputParser
import brainpy.tools as Tools
from . import BaseAgent
from typing import List

NEW_PERFIX = """
You are نگار ,a chatbot that works in Gostareh Negar. Your primary goal is to answer customer questions about 
Gostareh Negar and its products. 
You are designed to be able to assist with a wide range of tasks, from answering simple questions to providing in-depth explanations and discussions on a wide range of topics. As a language model, Assistant is able to generate human-like text based on the input it receives, allowing it to engage in natural-sounding conversations and provide responses that are coherent and relevant to the topic at hand.
Overall, you are a powerful system that can help with a wide range of tasks and provide valuable insights and information on a wide range of topics. Whether one need help with a specific question or just want to have a conversation about a particular topic, you are here to assist.
When answering follow these rules:
1. Always answer in Persian. 
2. When a user asks "do you have x", check if x is a product of Gostareh Negar.
3. When a user wants to chat with you, in this case forget about tools use your own capablity to be funny and try to entertain her.
"""


class MyParser(ConvoOutputParser):

    def get_format_instructions(self) -> str:
        """Returns formatting instructions for the given output parser."""
        # return FORMAT_INSTRUCTIONS
        return FORMAT_INSTRUCTIONS\
            .replace(
                "Use this if you want to respond directly to the human.",
                "Use this if you want to respond directly to the human. You should ALWAYS respond to human only in Persian Language. ")\
            .replace(
                " You should put what you want to return to use here",
                " You should put what you want to return to user here. Remember this should only be in Persian Language. ")

    def parse(self, text: str):
        if not "action" in text:
            text = '{"action":"Final Answer","action_input":"'+text+'"}'

        result = super().parse(text)
        kk = result
        return result


class AsssitantConversationalChatAgent(BaseAgent):
    def __init__(self) -> None:
        super().__init__("ConversationalAgentBrain")
        pass

    async def Initialize():
        pass

    # async def run(self, ctx: AgentContext, tools: List[Tool] = []):
    #     try:
    #         response = await self._run(ctx, tools=tools)
    #     except Exception as exp:
    #         response = ctx.creat_reply_from_error(exp)
    #     return ctx

    async def _run(self, ctx: AgentContext, tools: List[Tool] = []):
        memory = ctx.get_memory()
        llm = ctx.get_llm()
        if tools == None or len(tools) == 0:
            tools = [Tools.get_currentdate_tool(),
                     Tools.get_timeoffset_tool(),
                     Tools.get_schedulemeeting_tool()

                     ]
        _prefix = NEW_PERFIX  # PREFIX
        _suffix = SUFFIX
        # kwargs={"system_message":_prefix,"human_message":_suffix}
        kwargs = {"output_parser": MyParser(), "system_message": NEW_PERFIX}
        
        agent_chain = initialize_agent(tools, llm,
                                       agent=AgentType.ZERO_SHOT_REACT_DESCRIPTION,
                                       verbose=True,
                                        memory=memory,
                                        agent_kwargs={})
        response = agent_chain(ctx.request.Input)
        ctx.create_reply(response=response)

        return ctx

        # initialize memory
        # initialize llm
