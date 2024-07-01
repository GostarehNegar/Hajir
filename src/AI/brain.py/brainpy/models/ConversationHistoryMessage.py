from pydantic import BaseModel


from typing import Optional


class ConversationHistoryMessage(BaseModel):
    From: Optional[str] = ""

    Content: Optional[str] = ""

    def is_human(self):
        return self.From == "Human"

    @staticmethod
    def from_human_message(message: str):
        result = ConversationHistoryMessage()
        result.From = "Human"
        result.Content = message
        return result

    @staticmethod
    def from_ai_message(message: str):
        result = ConversationHistoryMessage()
        result.From = "AI"
        result.Content = message
        return result