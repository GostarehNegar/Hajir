
from langchain.chains import RetrievalQA
from langchain.agents import Tool
from brainpy.knowledge import Knowledge

def retreival_qa_tool(llm, kb:Knowledge):
    qa = RetrievalQA.from_chain_type(
            llm=llm,
            chain_type="stuff",
            retriever=kb.as_retriever())
    return  Tool(
                name='Knowledge Base',
                func=qa.run,
                description=(
                    'use this tool when answering general knowledge queries to get '
                    'more information about the topic'
                )
            )
