
from brainpy.agent.agentcontext import AgentContext
from brainpy.knowledge import Knowledge
from brainpy.agent import get_agent
from langchain.chains import ConversationalRetrievalChain
from langchain_community.chat_models import ChatOpenAI
from langchain.memory import ConversationBufferMemory, ConversationSummaryMemory
from langchain.memory.chat_memory import BaseChatMemory
from langchain.prompts.prompt import PromptTemplate
import brainpy.library.utils as utils
from brainpy.config import config
import brainpy.Prompts as Prompts


class Brain():
    chain: ConversationalRetrievalChain = None
    memory: BaseChatMemory = None

    def __init__(self):
        pass

    async def initialize(self, kn: Knowledge):
        self.knowlege = kn
        llm = ChatOpenAI()

        if (config.memory == "ConversationSummaryMemory"):
            self.memory = ConversationSummaryMemory(
                llm=llm, memory_key="chat_history", return_messages=True)
        else:
            self.memory = ConversationBufferMemory(
                llm=llm, memory_key="chat_history", return_messages=True)
        chain_type_kwargs = {"prompt": Prompts.get_qa_prompt()}
        self.chain = ConversationalRetrievalChain.from_llm(llm,
                                                           chain_type="stuff",
                                                           retriever=self.knowlege.as_retriever(),
                                                           verbose=True,
                                                           memory=self.memory,
                                                           combine_docs_chain_kwargs=chain_type_kwargs,
                                                           condense_question_prompt=Prompts.get_condense_question_prompt())

    def _to_serializable_memory(self, mem: BaseChatMemory):
        messages = []
        summary = ""
        _type_name = type(mem).__name__

        for m in mem.chat_memory.messages:

            messages.append({"type": type(m).__name__, "content": m.content})
            # if type(m).__name__.startswith("Human"):
            #     messages.append({"From":"Human","content":m.content})
            # else:
            #     messages.append({"From":"AI","content":m.content})
        if _type_name == "ConversationSummaryMemory":
            summary = mem.buffer
        return {"type": _type_name, "summary": summary, "messages": messages}

    def get_memory(self, llm, mem):

        if mem == None:
            result = ConversationBufferMemory(
                llm=llm, memory_key="chat_history", return_messages=True)
        else:
            memory_type = mem["type"]
            result = ConversationBufferMemory(
                llm=llm, memory_key="chat_history", return_messages=True)
            summary = mem["summary"]
            messages = mem["messages"]
            for m in messages:
                if m["type"] == "HumanMessage":
                    result.chat_memory.add_user_message(m["content"])
                else:
                    result.chat_memory.add_ai_message(m["content"])
        return result

    def get_llm(self, refresh: bool = False):
        return ChatOpenAI(model="gtp-4o")

    async def make_reply(self, ctx:AgentContext)->AgentContext:
        utils.logger.info("MakeReply Starts")
        # self.chain.memory.clear()
        llm = self.get_llm()
        request = ctx.request
        input = request.Input
        memory = ctx.get_memory()
        chain_type_kwargs = {"prompt": Prompts.get_qa_prompt()}
        chain = ConversationalRetrievalChain.from_llm(llm,
                    chain_type="stuff",
                    retriever=self.knowlege.as_retriever(),
                    verbose=True,
                    memory=memory,
                    combine_docs_chain_kwargs=chain_type_kwargs,
                    condense_question_prompt=Prompts.get_condense_question_prompt())
        response = chain(input)
        utils.logger.info("Finished Making Reply")
        ctx.create_reply(response)

        return ctx
    async def MakeReply(self, request):
        utils.logger.info("MakeReply Starts")
        # self.chain.memory.clear()
        llm = self.get_llm()
        input = request["Input"]
        memory = self.get_memory(llm, request["Memory"])
        chain_type_kwargs = {"prompt": Prompts.get_qa_prompt()}
        chain = ConversationalRetrievalChain.from_llm(llm,
                    chain_type="stuff",
                    retriever=self.knowlege.as_retriever(),
                    verbose=True,
                    memory=memory,
                    combine_docs_chain_kwargs=chain_type_kwargs,
                    condense_question_prompt=Prompts.get_condense_question_prompt())

        response = chain(input)
        utils.logger.info("Finished Making Reply")
        return {'answer': response['answer'], "memory": self._to_serializable_memory(memory)}
        return response
    

    
    
