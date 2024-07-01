from chromadb.server.fastapi import FastAPI
import chromadb
import chromadb.config
import asyncio
import uvicorn
settings = chromadb.config.Settings(is_persistent= True,persist_directory="./db")
server = FastAPI(settings)
app = server.app
async def main():
    config = uvicorn.Config("main:app", port=8009, log_level="info",workers=1)
    server = uvicorn.Server(config)
    await server.serve()


if __name__ == "__main__":
    asyncio.run(main())