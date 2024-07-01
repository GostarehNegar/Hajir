from langchain.chains.conversational_retrieval.prompts import CONDENSE_QUESTION_PROMPT,QA_PROMPT
from langchain.prompts.prompt import PromptTemplate

 

_custom_template = """Given the following conversation and a follow up question, rephrase the follow up question to be a standalone question. At the end of standalone question add this 'Answer the question in Persian language.' If you do not know the answer reply with 'I am sorry'.
Chat History:
{chat_history}
Follow Up Input: {question}
Standalone question:"""
CUSTOM_CONDENSE_QUESTION_PROMPT = PromptTemplate.from_template(_custom_template)

prompt_template = """Use the following pieces of context to answer the question at the end. If you don't know the answer, just say that you don't know, don't try to make up an answer.
{context}
Question: {question}
Answer in Persian:"""
CUSTOM_QA_PROMPT= PromptTemplate(template=prompt_template, input_variables=["context", "question"]) 

def get_condense_question_prompt():
    return  CUSTOM_CONDENSE_QUESTION_PROMPT if 1==0 else CONDENSE_QUESTION_PROMPT
def get_qa_prompt():
    return CUSTOM_QA_PROMPT if 1==1 else QA_PROMPT
