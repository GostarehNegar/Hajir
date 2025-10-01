from chromadb.server.fastapi import FastAPI
import chromadb
import chromadb.config
import asyncio
import uvicorn
from scraper import scraper
from models import AddKbDocumetRequest
from app import app
from bus import bus
import api
import logging
FORMAT = '%(levelname)s  %(message)s'
logging.basicConfig(level=logging.INFO, format=FORMAT)

async def main():
    await app.initialize()
    # Note FastApi app is actually preserved
    # in app.api
    config = uvicorn.Config("main:app.api", host='0.0.0.0',
                            port=8009, log_level="info", workers=1)
    server = uvicorn.Server(config)
    await server.serve()


if __name__ == "__main__":
    asyncio.run(main())
