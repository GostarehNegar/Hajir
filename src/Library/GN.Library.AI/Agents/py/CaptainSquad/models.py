"""
     Vesion 1.0
"""
from pydantic import BaseModel
from typing import List, Optional, Dict, Any
from dataclasses import dataclass


class Conversation(BaseModel):
    Query: str = "ll"


class AgentHearBeat(BaseModel):
    Name: str
    Description: str


class ListAgentsResponse(BaseModel):
    Agents: List[AgentHearBeat]


class ChatMessage(BaseModel):
    role: str = None,
    content: List[Any] = []


class SessionContext(BaseModel):
    SessionId: str
    UserId: str
    Parameters: Optional[Dict[str, str]] = None


class AgentRequest(BaseModel):
    input_text: str = None
    chat_history: List[ChatMessage] = None
    context: Optional[SessionContext] = None


class AgentResponse(BaseModel):
    text: str = None


class Contact(BaseModel):
    FirstName: str = None
    LastName: str = None
    Mobile: str = None


class LLMParameters(BaseModel):
    Url: str = None
    ApiKey: str = None
    Name: Optional[str] = None
    Model: str = None


class GetAvailableLLMsResponse(BaseModel):
    LLMs: List[LLMParameters] = None
    Default: LLMParameters = None


@dataclass
class ToolParameter:
    """Represents a tool parameter with its metadata."""
    name: str
    type: str
    description:str
    required: bool = True
    default: Any = None


@dataclass
class ToolMetadata:
    """Metadata describing a NATS-based tool."""
    name: str
    description: str
    subject: str
    parameters: List[ToolParameter]
    """
        sample: {"type": "object", "description": "Statistics about the text"}
    """
    returns: Dict[str, Any] 


@dataclass
class ToolRequest:
    parameters: Dict[str, Any]
