from app import app
from lib import bus
import asyncio
import uvicorn
import logging
from dotenv import load_dotenv
FORMAT = '%(levelname)s  %(message)s'
logging.basicConfig(level=logging.INFO, format=FORMAT)
load_dotenv()


@app.get("/api/ping")
def ping():
    return "pong"

async def main():
    await app.run()
    # Note FastApi app is actually preserved
    # in app.api
    config = uvicorn.Config("main:app", host='0.0.0.0', port=7980,
                             log_level="info", workers=1)
    server = uvicorn.Server(config)
    await server.serve()


if __name__ == "__main__":
    asyncio.run(main())
