
from .BaseAgent import BaseAgent
from .ConversationalAgentBrain import ConversationalChatAgent
from .AssistantAgent import AsssitantConversationalChatAgent


def get_conversational_chat():
    pass


all_agents = {"conversational": ConversationalChatAgent(
), "assisant": AsssitantConversationalChatAgent()}


def get_agent(name: str) -> BaseAgent:
    return all_agents["conversational"]
