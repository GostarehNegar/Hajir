from langchain.chains import RetrievalQA

from langchain.agents import Tool
from langchain.tools import BaseTool
from brainpy.knowledge import Knowledge
INFO="""
Your name is هزیر
You are 28 years old.
You work in هزیر صنعت.
You love Cinema and Comic books.
Your favorite singer is Celin Dion.
You are fluent in Persian and English.

"""


class BotInfoTool(BaseTool):
        name='Your Profile'
        description="use this tool when you need tell information about yourself. "
        def _run(self, input:str):
             return INFO
        def _arun(self, input: str):
            raise NotImplementedError("This tool does not support async")
def get_bot_info_tool():
     return BotInfoTool()

             




def pricelist_tool(llm, kb: Knowledge):
    return PriceListTool()
