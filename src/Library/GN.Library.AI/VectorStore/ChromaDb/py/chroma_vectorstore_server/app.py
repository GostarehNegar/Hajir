import options
from chromadb.server.fastapi import FastAPI
import chromadb
import chromadb.config
import asyncio
import uvicorn
from scraper import scraper
from bus import bus, MsgContext
import logging
from langchain_community.embeddings import OpenAIEmbeddings
from langchain_community.vectorstores import Chroma
from langchain_community.document_loaders import DirectoryLoader, TextLoader, AsyncHtmlLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
import chromadb
from chromadb.config import Settings
from sentence_transformers import SentenceTransformer
from chromadb.utils import embedding_functions
import os
import options
import logging
from models import AddKbDocumetRequest
#logging.basicConfig(level=logging.INFO)
settings = chromadb.config.Settings(
    is_persistent=True, persist_directory="./db")
server = FastAPI(settings)
logger =logging.getLogger("app")

embeding_model = 'heydariAI/persian-embeddings'
# all-MiniLM-L6-v2
# "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"
# model = SentenceTransformer('all-MiniLM-L6-v2')
sentence_transformer_ef = embedding_functions.SentenceTransformerEmbeddingFunction(
    model_name=embeding_model
)

client = chromadb.PersistentClient(path=options.Db_Folder)
collection = client.get_or_create_collection(
    name="tamin-kb", embedding_function=sentence_transformer_ef)
text_splitter = RecursiveCharacterTextSplitter(
    chunk_size=1000, chunk_overlap=200)

class App():
    def __init__(self):
        self.api = server.app()
        pass

    async def initialize(self):
        if options.UseBus:
            await bus.initialize()
            await bus.subscribe("foo.test",self.foo_handler)
        logger.info("App Successfully Initialized.")
        return True

   

    async def foo_handler(self, msg: MsgContext):
        print(msg.msg.subject)
        pass


app = App()
