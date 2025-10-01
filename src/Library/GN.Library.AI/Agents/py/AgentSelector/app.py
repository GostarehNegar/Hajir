from fastapi import FastAPI
from bus import bus, MsgContext
from typing import List
from langchain_openai import ChatOpenAI
from langchain_core.prompts import PromptTemplate
from langchain.chains import LLMChain
from models import Conversation
import logging
server = FastAPI()

api_key = "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"
url="https://openrouter.ai/api/v1"

logger = logging.getLogger(__name__)
model = "openai/gpt-oss-120b"
template = """Question: {question}
Answer: Let's think step by step."""

class SimpleAgent():
    def __init__(self):
        pass
prompt = PromptTemplate(template=template, input_variables=["question"])

class App(FastAPI):
    async def initialize(self):
        bus.initialize()
        self.llm = ChatOpenAI(api_key=api_key,
                              base_url=url,
                              temperature=0.7, model=model)
    # if options.UseBus:
    #     await bus.initialize()
        await bus.subscribe("foo.test",self.foo_handler)
        

    # logger.info("App Successfully Initialized.")
        return True
    
    async def simple_answer(self,msg:MsgContext):


        pass

    async def foo_handler(self, msg: MsgContext):
        print(msg.msg.subject)
        c= msg.GetPayload(Conversation)
        print(c.Query)
        llm_chain = LLMChain(prompt=prompt, llm=self.llm)
        print(llm_chain.run(c.Query))
        pass

app = App()