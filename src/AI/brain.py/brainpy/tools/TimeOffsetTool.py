from langchain.chains import RetrievalQA

from langchain.agents import Tool
from langchain.tools import BaseTool, StructuredTool
import datetime 
StructuredTool.from_function
class TimeOffsetTool(BaseTool):
        name='Time Offset'
        description="use this tool to calculate time offset between any date and current date. "\
            "The input to this tool is the target date and time in ISO format."\
             "Remember that to use this tool you may need to use the Current Time tool. For example "\
             "if you need to get the date for tomorrow 5 AM first you need to know what is the current time. "
        
        
          # "Also note thate It is absolutely crucial to use the ISO date time format and time should always be included."\
            
        def _run(self, input:str):
             target = datetime.datetime.fromisoformat(input)
             return (datetime.datetime.now()- target).total_seconds()
             
        def _arun(self, input: str):
            raise NotImplementedError("This tool does not support async")
        
             




def get_timeoffset_tool():
    return TimeOffsetTool()
