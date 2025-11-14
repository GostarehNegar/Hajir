from pydantic import BaseModel, Field
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

class ProductSearchInput(BaseModel):
    """Input schema for searching products."""
    name: str = Field(description="String to search in product names")

class Product(BaseModel):
    """A product object."""
    id: str = None
    name: str =None
    category: str | None 
    price: float = None
    in_stock: bool = False

class ProductSearchResult(BaseModel):
    """Result of a product search."""
    products: List[Product]=[]
    total_found: int =0
    category_searched: str=""