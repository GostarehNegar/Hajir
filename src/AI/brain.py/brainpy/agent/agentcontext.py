import brainpy.models.BrainModels as BrainModels

from brainpy.knowledge import Knowledge
from langchain_community.chat_models import ChatOpenAI
from langchain.memory import ConversationBufferMemory, ConversationSummaryMemory
from langchain.memory.chat_memory import BaseChatMemory
class AgentContext:
    # request: BrainModels.BrainRequestModel = None
    # reply = BrainModels.BrainReplyModel
    # llm = None
    # knowlegde: Knowledge = None
    def __init__(self, req: BrainModels.BrainRequestModel = None, kb: Knowledge = None) -> None:
        self.request = req
        self.knowlegde = kb
        self.reply = None
        self.llm = None
        

    
    def get_knowlede(self):
        return self.knowlegde

    def get_llm(self, refresh: bool = False):
        if self.llm == None:
            self.llm = ChatOpenAI(model="gpt-4o")
        return self.llm

    def get_memory(self):
        llm = self.get_llm()
        mem = None if self.request.Conversation==None else self.request.Conversation.Memory
        if mem == None:
            result = ConversationBufferMemory(
                llm=llm, memory_key="chat_history", return_messages=True)
        else:
            result = ConversationBufferMemory(
                llm=llm, memory_key="chat_history", return_messages=True)
            # return result
            for m in mem.Messages:
                if m.is_human():
                    result.chat_memory.add_user_message(m.Content)
                else:
                    result.chat_memory.add_ai_message(m.Content)
        return result

    def create_reply(self, response):
        self.reply = BrainModels.BrainReplyModel.from_response(response)
        self.reply.Conversation.Id = self.request.Conversation.Id

    def creat_reply_from_error(self,err:Exception):
        self.reply = BrainModels.BrainReplyModel.from_error(err)
        self.reply.Conversation.Id = self.request.Conversation.Id



        
