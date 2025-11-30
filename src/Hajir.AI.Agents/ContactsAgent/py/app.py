from fastapi import FastAPI
from lib import bus, MsgContext, BaseAgent, CancellationToken, Subjects
from typing import List, Optional, Dict
import models
import logging
from langchain_classic.tools import tool
from openai import OpenAI
from tenacity import retry, stop_after_attempt, wait_exponential
from lib_langchain import toolsRegistry, LangchainAgent
import asyncio
import math


logger = logging.getLogger(__name__)

agent_name = "shima"
agent_description = """An agent to manage contacts and people in CRM system
                capable of:
                1. Searching for Contacts (Prople).
                """
agent_system_prompt = """
                    You are a helpful assistant with access to various tools. 
                    Use them when needed to answer questions accurately. 
                    You work as an assistant in Hajir Sanat. 
                    Your primary task is to give information Contacts using CRM system.
                    You can use tools to:
                    1. Search Contacts in CRM.
                    2. Create new Contacts.
                    IMPORTANT RULES:
                    You should absolutley follow these rules:
                    1. Use 'search_contacts' tool to search for contacts (people).
                    2. Yous should try to use the tools whenver you are asked to retry.
                    
                    NEVER TRY TO GENERATE INFORMATION BY YOUR SELF. 
                    
                    Always respond in Persian (Farsi) when the user speaks in Persian.
                    
                    """


@tool
async def calculator(expression: str) -> str:
    """Evaluates a mathematical expression. 
    Use this for any math calculations.

    Args:
        expression: A mathematical expression as a string (e.g., "2 + 2", "sqrt(16)")
    """
    await asyncio.sleep(1)
    try:
        # Use math functions if needed
        result = eval(expression, {"__builtins__": {}}, {
            "sqrt": math.sqrt,
            "pow": math.pow,
            "sin": math.sin,
            "cos": math.cos,
            "pi": math.pi,
        })
        return str(result)
    except Exception as e:
        return f"Error calculating: {str(e)}"


class Agent(LangchainAgent):
    def __init__(self):
        super().__init__(agent_name, agent_description, agent_system_prompt)

    def select_tool(t: models.ToolMetadata) -> bool:
        return True

    async def getTools(self, ctx: models.SessionContext):
        _tools = await toolsRegistry.getTools(True, self.select_tool, context=ctx)
        tool_names = ["search_contacts","search_accounts","account_information","create_account","send_sms"]
        result = []
        for tool_name in tool_names:
            tool = toolsRegistry.getToolByName(tool_name)
            if tool == None:
                logger.warning(f"{tool_name} tool not found.")
            else:
                logger.info(f"Tool {tool_name} added.")
                result.append(tool)
        

        return result


agent = Agent()


class App(FastAPI):
    async def run(self):
        await agent.start(bus)


app = App()
