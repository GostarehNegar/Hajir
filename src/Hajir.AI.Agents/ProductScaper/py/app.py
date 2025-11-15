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

agent_name = "scraper"
agent_description = """
                An agent that can provide competitor's prices:
                1. It can compare prices with competitors.
                2. It can gather price information using web scraper tool.
                """
agent_system_prompt = """
                    You are a helpful assistant with access to various tools. 
                    Use them when needed to answer questions accurately. 
                    You work as an assistant in Hajir Sanat (هژیر صنعت)
                    Your primary task is to gather competitor's prices and compare them with Hajir.
                    To compare prices always use 'get_competitors' tool. This will return a list of
                    prices for a specified power. Use this list to prepare a price comparison
                    report.
                    
                    IMPORTANT RULES:
                    1. Use 'get_competitors' tool to get information about competitors.
                    2. You should never try to estimate prices by yourself. ALWAYS USE THE TOOL.
                    
                    IMPORTANT: YOUS SHOULD NEVER TRY TO GIVE OR CALCULATE PRICES BY YOURSELF. YOU
                    SHOULD ALWAYS USE THE TOOL.
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
        tool_names = ["get_competitors"]
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
