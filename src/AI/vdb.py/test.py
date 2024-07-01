
import asyncio
import chromadb

async def test1():
    print('here')
    client = chromadb.HttpClient(host='localhost', port=8009)

    collections = client.list_collections()
    collection = client.get_or_create_collection("langchain")
    count = collection.count()

    
    
    collection.add(
    documents=["lorem ipsum...", "doc2", "doc3"],
    metadatas=[{"chapter": "3", "verse": "16"}, {"chapter": "3", "verse": "5"}, {"chapter": "29", "verse": "11"}],
    ids=["id1", "id2", "id3"]
    )




    pass

if __name__ == "__main__":
    asyncio.run(test1())