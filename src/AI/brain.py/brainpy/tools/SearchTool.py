from langchain.chains import RetrievalQA

from langchain.agents import Tool
from langchain.tools import BaseTool
from brainpy.knowledge import Knowledge
from langchain_community.tools import DuckDuckGoSearchRun

search = DuckDuckGoSearchRun()


class DuckDuckGoSearchTool(BaseTool):
        name='DuckDuckGo Search'
        description="Useful for when you need to do a search on the internet to find information that another tool."

        def _run(self, input:str):
             return search.run(input)
        def _arun(self, input: str):
            raise NotImplementedError("This tool does not support async")
def get_search_tool():
     return DuckDuckGoSearchTool()

             




