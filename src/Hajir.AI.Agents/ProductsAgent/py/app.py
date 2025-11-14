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

agent_name = "products_agent"
agent_description = """
                An agent that can provide informaion about products and prices:
                1. It can list for profucts.
                2. It can give prices
                """
agent_system_prompt = """
                    You are a helpful assistant with access to various tools. 
                    Use them when needed to answer questions accurately. 
                    You work as an assistant in Hajir Sanat. 
                    Your primary task is to give information about products and their prices.
                    IMPORTANT RULES:
                    1. Use 'search_products' tool to get information about products.
                    2. It may happen that the user is selecting a number of products to put an order
                       in this case try to calculate the totals.
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
        result = []
        tool = toolsRegistry.getToolByName("search_products")
        if tool==None:
            logger.warning(f"search_products tool not found.")
        else:
            result.append(tool)
        result.append(calculator)
            
        return result

agent = Agent()


class App(FastAPI):
    async def run(self):
        await agent.start(bus)
app = App()
