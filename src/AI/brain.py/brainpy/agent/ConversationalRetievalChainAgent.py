from . import BaseAgent
from brainpy.agent.agentcontext import AgentContext
from typing import List
from langchain.agents.tools import Tool
from brainpy import utils
from brainpy.config import config
import brainpy.Prompts as Prompts
from langchain.chains import ConversationalRetrievalChain
class ConversationalRetievalChainAgent(BaseAgent):
    async def run(self, ctx: AgentContext, tools: List[Tool] = []):
        
        pass
    async def _run(self, ctx: AgentContext, tools: List[Tool] = []):
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
