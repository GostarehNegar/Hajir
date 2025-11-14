from fastapi import FastAPI
from lib import bus, MsgContext, BaseAgent, CancellationToken, Subjects
from typing import List, Optional, Dict
import models
import logging

from openai import OpenAI
from tenacity import retry, stop_after_attempt, wait_exponential
from lib_langchain import toolsRegistry, LangchainAgent


logger = logging.getLogger(__name__)

agent_name = "Opportunist"
agent_description = """
                An agent that can create/manage sale opportunities:
                1. It can show a list of active opportunities that can be filtered.
                2. It can create a new opportunity.
                """
agent_system_prompt = """
                    You are a helpful assistant with access to various tools. 
                    Use them when needed to answer questions accurately. 
                    You work as an assistant in Hajir Sanat. 
                    Your primary task is to help users create/manage sales opportunities in the CRM system. 
                    Also note to create an opportunity it is often required to specify a client
                    or contact. In these cases you can use the specific tool to to find the contact.
                    IMPORTANT RULES:
                    1. Always get user approvals before registring phonecalls.
                    2. Make sure to use create_opportunity tool and also display the tool results to user.
                       Specially check the Successfull status and the returned OpportunityI Id. 
                    3. When there are multiple contacts show them to the user so that she can pick one of them.
                    4. YOU SHOULD ALWAYS USE 'create_opportunity' TOOL TO CREATE OPPORTUNITY
                    
                    """


class Agent(LangchainAgent):
    def __init__(self):
        super().__init__(agent_name, agent_description, agent_system_prompt)

    def select_tool(t: models.ToolMetadata) -> bool:
        return True

    async def getTools(self, ctx: models.SessionContext):
        _tools = await toolsRegistry.getTools(True, self.select_tool, context=ctx)
        result = []
        tool = toolsRegistry.getToolByName("search_contacts")
        if tool==None:
            logger.warning(f"search_contacts tool not found.")
        else:
            result.append(tool)
        tool = toolsRegistry.getToolByName("create_opportunity")
        if tool==None:
            logger.warning(f"create_opportunity tool not found.")
        else:
            result.append(tool)
        tool = toolsRegistry.getToolByName("list_opportunities")
        if tool==None:
            logger.warning(f"list_opportunities tool not found.")
        else:
            result.append(tool)
            
        return result

agent = Agent()


class App(FastAPI):
    async def run(self):
        await agent.start(bus)
app = App()
