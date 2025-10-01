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
print("db start")
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
print("db fin")

def query(q: str, no: int = 10):
    return collection.query(query_texts=[q], n_results=no)

def add_document(doc: AddKbDocumetRequest):
    collection.add(documents=[doc.content],
                   metadatas=[{"source": doc.source, "name": doc.name}],
                   ids=[doc.id])
    pass
def rebuild():
    logging.info(
        f"Start rebuilding vectore db. Document Path:{options.Documents_FOLDER}")
    folder = options.Documents_FOLDER
    for f in os.listdir(folder):
        file_name = os.path.join(folder, f)
        source = None
        try:
            if not os.path.isfile(os.path.join(folder, f)) or not f.endswith(".txt"):
                continue
            with open(file_name, "r", encoding='utf-8') as text_file:
                source = text_file.read()
            splitted = text_splitter.split_text(source)
            i = 0
            for text in splitted:
                id = id + 1
                collection.add(documents=[text],
                               metadatas=[{"url": f}],
                               ids=[f"{f}-{i}"]
                               )
        except Exception as e:
            print('Error')

    pass
