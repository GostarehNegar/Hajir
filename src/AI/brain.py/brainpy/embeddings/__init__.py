
from langchain_community.embeddings import OpenAIEmbeddings
from langchain.embeddings.base import  Embeddings
from brainpy.library import utils

def get_embedding(name:str=None)->Embeddings:
    return OpenAIEmbeddings(openai_api_key=utils.get_openai_key())











