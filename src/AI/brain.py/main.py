from fastapi import FastAPI
from fastapi.staticfiles import StaticFiles
import uvicorn
import asyncio
import logging
import sys
import getopt
from brainpy.config import config
from brainpy.app import app
from brainpy.knowledge import Knowledge
api = app.api
FORMAT = '%(levelname)s  %(message)s'
logging.basicConfig(level=logging.INFO, format=FORMAT)

def apply_args():
    argumentList = sys.argv[1:]
    long_options = ["Help", "url=", "name="]
    options = "hun:"
    try:
        # Parsing argument
        arguments, values = getopt.getopt(argumentList, options, long_options)

    # checking each argument
        for currentArgument, currentValue in arguments:

            print(currentArgument, currentValue)
            if currentArgument in ("-h", "--Help"):
                print("Displaying Help")

            elif currentArgument in ("-u", "--url"):
                url = currentValue
                print(currentValue)

            elif currentArgument in ("-n", "--name"):
                name = currentValue
                print(("Enabling special output mode (% s)") % (currentValue))

    except getopt.error as err:
        print(str(err))


async def main():
    # apply_args()
    # print("url=",url)
    # print("name=",name)

    await app.initialize()
    if 1 == 0:
        k = Knowledge()
        await k.initialize()
        await k.rebuild()
        print("rebuilded")
        return
    # k = Knowledge()
    # await k.initialize()
    # docs = k.as_retriever().aget_relevant_documents("فهرست محصولات")
    config = uvicorn.Config("main:api", port=8005, log_level="info", workers=1)
    server = uvicorn.Server(config)
    await server.serve()


if __name__ == "__main__":
    asyncio.run(main())
