from langchain.agents import AgentExecutor, create_tool_calling_agent
from langchain.tools import tool, StructuredTool
from langchain_core.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain_core.messages import HumanMessage, AIMessage
from langchain_openai import ChatOpenAI
from pydantic import BaseModel, Field
from typing import List, Optional, Dict, Any
import math
from lib import bus, CancellationToken, MsgContext, Subjects,BaseAgent
import models
import logging
import asyncio
logger = logging.getLogger(__name__)

agnet_name = "shima"
agnet_description="""
                An agent that can search for people and companies. 
                It searches the comapny CRM for Contacts and Accounts. """
agent_system_prompt="""
                    You are a helpful assistant with access to various tools. 
                    Use them when needed to answer questions accurately. 
                    Specially you may use
                    1. search_contacts tool to search for contacts.
                    2. You may use search_contacts to List contacts with a name
                    3. از ابزار search_contacts برای جستجوی مخاطبین استفاده کن
                    """
class Tools:
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
    async def search_contacts(name: str) -> List[models.Contact]:
        """Search for contacts (people) using the given string """
        # Mock data - in real implementation, this would query a database
        await asyncio.sleep(1)
        logger.info(f"Searching for contact {name}")
        repl = await bus.request('search_contact',{'search':name})
        ret = repl.GetPayload( List[models.Contact])
        return ret
    contact_search_tool = StructuredTool.from_function(
        #func=  search_products,
        coroutine=search_contacts,
        name="search_contacts",
        description="Search for contacts and people.",
        #args_schema=models.ProductSearchInput,
        return_direct=False
    )
    Tools=[calculator,contact_search_tool]
    
    pass

class ContactAgent(BaseAgent):

    def __init__(self, ):
        self._executor:AgentExecutor = None
        super().__init__(agnet_name,agnet_description)
    def ToMessage(self, msg:models.ChatMessage):
        return HumanMessage(msg.content[0].get('text')) if msg.role=='user' else AIMessage(msg.content[0].get('text'))
    
    async def executor(self,refresh:bool=False):
        if self._executor is None:
            url ="https://api.deepseek.com"
            api_key = "sk-fec93ba732c046b38b35263b0a4c004d" #"sk-3b8842c4b8de41b48ad350662886e849"
            GPT_OSS ="deepseek-chat"
            llm = ChatOpenAI(model=GPT_OSS, temperature=0, base_url=url,api_key=api_key)
            prompt = ChatPromptTemplate.from_messages([
                    ("system", agent_system_prompt),
                    MessagesPlaceholder(variable_name="chat_history"),
                    ("human", "{input}"),
                    ("placeholder", "{agent_scratchpad}"),
                ])
            
            agent = create_tool_calling_agent(llm, Tools.Tools, prompt)
            self._executor = AgentExecutor(
                agent=agent,
                tools=Tools.Tools,
                verbose=True,
                handle_parsing_errors=True
            )
        return self._executor

        
    async def reply(self, req: models.AgentRequest):
        logger.info(f"Shima starts.")
        _history = [self.ToMessage(msg)
                    for msg in req.chat_history]
        ex = await self.executor()
        response = await ex.ainvoke({
            "input": req.input_text,
            "chat_history": _history
            })
        
        return models.AgentResponse(text=response['output'])
    

shima = ContactAgent()