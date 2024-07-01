from langchain.chains import RetrievalQA

from langchain.agents import Tool
from langchain.tools import BaseTool
from brainpy.knowledge import Knowledge
from langchain_community.tools import DuckDuckGoSearchRun

search = DuckDuckGoSearchRun()


class BatteryCalculatorTool(BaseTool):
        name='Battery Calculator Search'
        description="Useful for when you need to calculate the required number of batteries."\
        "To do so you should specify the number of computers and the required backup time in hours seperated by comma."\
        "PLEASE NOTE THAT you should only give a list of integer values. If you do not have those quantities"\
        " first try to ask them. Also note that backup time should always be in hours, therefore ou may need "\
        "to convert from minutes to hours."

        def _run(self, NumberOfComputersAndBackupTime:str):
             print("*****************************",NumberOfComputersAndBackupTime)
             items = NumberOfComputersAndBackupTime.split(",")

             return int(items[0])* float(items[1])
             return 15
        def _arun(self, input: str):
            raise NotImplementedError("This tool does not support async")
def get_batterycalculator_tool():
     return BatteryCalculatorTool()

             




