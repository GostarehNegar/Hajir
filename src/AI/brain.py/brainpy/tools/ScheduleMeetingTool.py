from langchain.chains import RetrievalQA

from langchain.agents import Tool
from langchain.tools import BaseTool, StructuredTool
import datetime 

class ScheduelMeetingTool(BaseTool):
        name='Schedule Meeting'
        description="use this tool when you want to schedule a meeting. "\
        "You should use following steps:"\
        "1. Use 'Current Time' tool to get current time."\
        "2. Use Time Offset tool to get the time offset from current time."\
        "3. Use this tool with the calulated timeoffset followed by a comma seprated list of attendees."
        
        
            
        def _run(self, input:str):
             target = datetime.datetime.fromisoformat(input)
             return (datetime.datetime.now()- target).total_seconds()
             
        def _arun(self, input: str):
            raise NotImplementedError("This tool does not support async")
        
             




def get_schedulemeeting_tool():
    return ScheduelMeetingTool()
