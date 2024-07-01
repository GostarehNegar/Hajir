from langchain_community.embeddings import OpenAIEmbeddings
from langchain_community.vectorstores import Chroma
from langchain_community.document_loaders import DirectoryLoader, TextLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
import chromadb
from chromadb.config import Settings
import brainpy.embeddings
from brainpy import config
from brainpy.library import utils
from brainpy.config import ChromaSettings


class Knowledge():
    db: Chroma
    persist_directory: str = "db"

    def __init__(self, settings: ChromaSettings = None):
        # self.db = None
        self.settings = settings
        pass

    def _chroma_settings(self) -> Settings:
        result = chromadb.get_settings()
        result.persist_directory = self.settings.persist_directory 
        # result.persist_directory = "db"
        result.is_persistent = True
        return result

    def _client(self):
        settings = self._chroma_settings()
        if self.settings.is_http_client():
            return chromadb.HttpClient(host=self.settings.host, port=self.settings.port)
        else:
            return chromadb.Client(settings)
    def _chroma(self) -> Chroma:
        client = self._client()
        embedding = brainpy.embeddings.get_embedding()
        persist_directory = "db"  # s brainpy.config.dataPath
        result = Chroma(persist_directory=self.settings.persist_directory,
                        collection_name="gnco",
                        embedding_function=embedding,
                        client_settings=self._chroma_settings(),
                        client=client)
        return result

    async def initialize(self, settings: ChromaSettings = None):
        self.settings = settings if settings else config.chroma
        self.db = self._chroma()
        # Chroma(persist_directory=persist_directory,embedding_function=embedding)
        retriever = self.db.as_retriever()

        # docs = retriever.get_relevant_documents("پرنیان")
        # print(len(docs))

        pass

    async def rebuild(self):
        utils.getLogger().info("here................")
        print("here...................")
        loader = DirectoryLoader(
            './docs/', glob='./*.txt', loader_cls=TextLoader, loader_kwargs={'encoding': 'utf8'})
        documents = loader.load()
        text_splitter = RecursiveCharacterTextSplitter(
            chunk_size=1000, chunk_overlap=200)
        texts = text_splitter.split_documents(documents)
        embedding = OpenAIEmbeddings()
        client = chromadb.HttpClient(port=8009)

        vectordb = Chroma.from_documents(documents=texts,
                                         embedding=embedding,
                                         collection_name="gnco",
                                         client=client,
                                         client_settings=self._chroma_settings(),
                                         persist_directory=self.persist_directory)
        print(len(texts))
        vectordb.persist()

    def as_retriever(self):
        return self.db.as_retriever()
