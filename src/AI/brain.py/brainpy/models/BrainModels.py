from pydantic import BaseModel
from typing import Optional
from typing import List
from brainpy.models.ConversationHistoryMessage import ConversationHistoryMessage

class ConversationMemoryModel(BaseModel):
    Messages: Optional[List[ConversationHistoryMessage]]=[]
    Temp:Optional[str]


    def add_human_message(self, message: str):
        if self.Messages == None:
            self.Messages = []
        self.Messages.append(
            ConversationHistoryMessage.from_human_message(message))

    def add_ai_message(self, message: str):
        if self.Messages == None:
            self.Messages = []
        self.Messages.append(
            ConversationHistoryMessage.from_ai_message(message))


class ConversationModel(BaseModel):
    """
        Conversation Id
    """
    Id: Optional[str]
    Memory: Optional[ConversationMemoryModel] = ConversationMemoryModel()


class BrainDataModel(BaseModel):
    format: Optional[str]


class BrainRequestModel(BaseModel):
    Conversation: Optional[ConversationModel]= ConversationModel()
    Input: Optional[str]


class BrainReplyModel(BaseModel):
    Conversation: Optional[ConversationModel] = ConversationModel()
    Output:str=""
    Status:int = 0
    Error:str = None

    @staticmethod
    def from_error(err:Exception):
        result = BrainReplyModel()
        result.Status = -1
        result.Error = (str)(err)
        return result
    @staticmethod
    def from_response(response):
        result = BrainReplyModel()
        # result.init()
        if "output" in response:
            result.Output = response["output"]
        result.Conversation.Memory.Temp="qqq"

        if "chat_history" in response:
            for m in response["chat_history"]:
                if (m.type == "human"):
                    result.Conversation.Memory.add_human_message(m.content)
                    aa = result.Conversation.Memory
                    bb = aa.Temp
                else:
                    result.Conversation.Memory.add_ai_message(m.content)
        return result
