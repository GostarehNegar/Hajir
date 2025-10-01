from pydantic import BaseModel
from typing import List, Optional, Dict, Any


class Conversation(BaseModel):
    Query: str = "ll"


class AgentHearBeat(BaseModel):
    Name: str
    Description: str


class ListAgentsResponse(BaseModel):
    Agents: List[AgentHearBeat]


class ChatMessage(BaseModel):
    role: str = None,
    content : List[Any] =[]


class AgentRequest(BaseModel):
    input_text: str = None
    user_id: str = None
    session_id: str = None
    chat_history: List[ChatMessage]= None
    additional_params: Optional[Dict[str, str]] = None


class AgentResponse(BaseModel):
    text: str = None
