import os
import sys
from langchain_community.document_loaders import DirectoryLoader, TextLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_community.embeddings import OpenAIEmbeddings
from langchain.vectorstores import Chroma
from langchain.chains import ConversationalRetrievalChain
from langchain_community.chat_models import ChatOpenAI
from langchain.memory import ConversationBufferMemory
print(os.path.dirname(sys.executable))
# https://colab.research.google.com/drive/1gyGZn_LZNrYXYXa-pltFExbptIe7DAPe?usp=sharing
"""
"""

os.environ["OPENAI_API_KEY"]='KEY'
persist_directory = 'db'

def MakeDb():
    loader = DirectoryLoader('./docs/',glob='./*.txt',loader_cls=TextLoader)

    documents = loader.load()
    text_splitter = RecursiveCharacterTextSplitter(chunk_size=1000, chunk_overlap=200)
    texts = text_splitter.split_documents(documents)
    print(len(texts))
    

## here we are using OpenAI embeddings but in future we will swap out to local embeddings
    embedding = OpenAIEmbeddings()

    vectordb = Chroma.from_documents(documents=texts, 
                                    embedding=embedding,
                                    persist_directory=persist_directory)
    vectordb.persist()
    vectordb = None
    vectordb = Chroma(persist_directory=persist_directory, 
                    embedding_function=embedding)
    retriever = vectordb.as_retriever()
    docs = retriever.get_relevant_documents("شیرپوینت چیست")
    print(docs[0])
    return "ok"
def test():
    embedding = OpenAIEmbeddings()
    vectordb = None
    vectordb = Chroma(persist_directory=persist_directory, 
                    embedding_function=embedding)
    retriever = vectordb.as_retriever()
    docs = retriever.get_relevant_documents("شیرپوینت چیست")
    print(len(docs))
    return "ok"
def test1():
    embedding = OpenAIEmbeddings()
    vectordb = Chroma(persist_directory=persist_directory, 
                    embedding_function=embedding)
    memory = ConversationBufferMemory(memory_key="chat_history", return_messages=True)
    
    chain = ConversationalRetrievalChain.from_llm(ChatOpenAI(),
                                                   chain_type="stuff",
                                                   retriever=vectordb.as_retriever(),
                                                   verbose=True,
                                                   memory=memory)
    # chain.return_source_documents = True
    # chain.return_generated_question = True
    #print(chain.combine_docs_chain.llm_chain.prompt)
    response = chain("امکانات فارسی ساز شیرپوینت چیست ")

    print(memory.chat_memory)
    
    print(response)
test1()
    