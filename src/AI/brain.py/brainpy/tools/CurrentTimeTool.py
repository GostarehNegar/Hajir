from langchain.chains import RetrievalQA

from langchain.agents import Tool
from langchain.tools import BaseTool
import datetime 

class CurrentTimeTool(BaseTool):
        name='Current Time'
        description="use this tool to get current date and time. The input to this tool will always be 'current time'"
        def _run(self, input:str):
             return datetime.datetime.now().isoformat()
             
        def _arun(self, input: str):
            raise NotImplementedError("This tool does not support async")
        
             




def get_currentdate_tool():
    return CurrentTimeTool()
