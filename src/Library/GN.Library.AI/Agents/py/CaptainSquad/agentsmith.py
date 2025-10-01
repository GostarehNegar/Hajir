from lib import bus, BaseAgent
from openai import OpenAI
from typing import List, Optional, Dict
from agent_squad.agents import Agent, AgentOptions, OpenAIAgent, OpenAIAgentOptions
from agent_squad.types import ConversationMessage, ParticipantRole
import models
import asyncio
import logging
api_key = "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"
url = "https://openrouter.ai/api/v1"
GPT_OSS = "openai/gpt-oss-120b"

client = OpenAI(base_url=url, api_key=api_key)
openai_agent = OpenAIAgent(OpenAIAgentOptions(
    name='OpenAI Agent',
    description='An agent that uses OpenAI API for responses and knows about Iran',
    api_key=api_key,
    client=client,
    model=GPT_OSS
))

logger = logging.getLogger("smith")


def ToConversationMessage(msg: models.ChatMessage) -> ConversationMessage:
    return ConversationMessage(role=ParticipantRole.USER if msg.role == 'user' else ParticipantRole.ASSISTANT,
                               content=msg.content)


class AgnetSmith(BaseAgent):
    def __init__(self, ):
        super().__init__("Smith",
                         'Generic agent that can generally chat with user about.')

    async def reply(self, req: models.AgentRequest):
        logger.info(f"Smith starts.")
        _history = [ToConversationMessage(msg)
                    for msg in req.chat_history]
        res = await openai_agent.process_request(req.input_text,
                                                 user_id=req.user_id,
                                                 session_id=req.session_id,
                                                 chat_history=_history,
                                                 additional_params=req.additional_params)
        logger.info(f"Smith finishes. {res.content[0]}")
        return models.AgentResponse(text=res.content[0].get('text'))


smith = AgnetSmith()
