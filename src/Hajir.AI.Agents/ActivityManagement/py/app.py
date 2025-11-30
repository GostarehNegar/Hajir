from fastapi import FastAPI
from lib import bus, MsgContext, BaseAgent, CancellationToken, Subjects
from typing import List, Optional, Dict
import models
import logging
from langchain_classic.tools import tool
from openai import OpenAI
from tenacity import retry, stop_after_attempt, wait_exponential
from lib_langchain import toolsRegistry, LangchainAgent
from datetime import datetime;

logger = logging.getLogger(__name__)

agent_name = "Miss_Daftary"
agent_description = """
                An agent that can manage and tracke user activities such as 
                Managing tasks, Meetings, Phonecalls and emails.
                It can record activities in the company CRM application. 
                Usage Examples:
                    Register Phonecalls
                    Create Reminders
                    Create/Send Emails 
                    Send SMS
                    Communicate contacts by sending messages
                    Arrange/Register Meetings
                """
agent_system_prompt = """
                    You are a helpful assistant with access to various tools. 
                    You MUST use tools for ALL actions. Never assume or claim to have used a tool without actually calling it. Always verify tool execution by waiting for the response before proceeding.
                    
                    Use them when needed to answer questions accurately. 
                    You work as an assistant in Hajir Sanat. 
                    Your primary task is to help users to manage their activities, specially:
                    1. Register activities such as registering phonecalls, meetings, emails.
                    2. Providing information about users activities, such as tasks.
                    You are provided with a set of tools for each of above tasks 
                    for example there is a tool to register a phone call, or send an sms.
                    Also note to register an activity it is often required to specify a client
                    or contact. For instance to register a phone call you need to specify the client 
                    who has called. In these cases you can use the specific tool to to find the contact.
                    IMPORTAN RULES
                    
                    1. Always get user approvals before registring phonecalls.
                    2. Make sure to use register_phonecalls and also display the results to user.
                    3. When there are multiple contacts show them to the user so that she can pick one of them.
                    4. When user wants you to tell something to some one you should try to send sms. You should
                        first find the contact and then send the message.
                    5. You should always user "send_sms" tool to send messages. The message returns a Tracking Code 
                       that you should provide to the user.
                    6. To create a reminder you should always call 'create_reminder'. 
                    
                    YOU SHOULD ALWAYS USE THE TOOLS. DO NOT DO THINGS BY YOURSELF.
                    """
@tool
def current_time(expression: str):
    """
        Returns Current Time 
    """
    return datetime.now()

class Agent(LangchainAgent):
    def __init__(self):
        super().__init__(agent_name, agent_description, agent_system_prompt)

    def select_tool(t: models.ToolMetadata) -> bool:
        return True

    async def getTools(self, ctx: models.SessionContext):
        _tools = await toolsRegistry.getTools(True, self.select_tool, context=ctx)
        return _tools
        result = []
        tool_names = ["search_contacts","register_phonecall","send_sms","create_reminder","mytasks","search_accounts"]
        result = []
        for tool_name in tool_names:
            tool = toolsRegistry.getToolByName(tool_name)
            if tool == None:
                logger.warning(f"{tool_name} tool not found.")
            else:
                logger.info(f"Tool {tool_name} added.")
                result.append(tool)
        result.append(current_time)
        return result

agent = Agent()


class App(FastAPI):
    async def run(self):
        await agent.start(bus)
app = App()
