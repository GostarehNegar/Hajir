from brainpy import bus
from brainpy.agent.ConversationalAgentBrain import ConversationalChatAgent
from brainpy.app import app
from threading import Thread
import asyncio
import time
from unittest import IsolatedAsyncioTestCase
import unittest
from brainpy import utils
from brainpy import config
from brainpy.knowledge import Knowledge
from brainpy import constants
from brainpy import constants
import brainpy

class TestKB(IsolatedAsyncioTestCase):
   
    async def test_bus_works(self):
        bus = brainpy.bus
        config = brainpy.config
        await bus.initialize(config.bus)
        await bus.publish(bus.name, {"hey":"hi"})
        




    async def test_config_works(self):
        config.save()
        

    async def test_knowlede_works(self):
        kn = Knowledge()
        await kn.initialize()
        

                           
    async def test_embeddings(self):
        await asyncio.sleep(1)

async def _async(s: str) -> str:
    await asyncio.sleep(1)
    return s.capitalize()


def _sync():

    try:
        res = utils.run_async(_async("result"))
        res = utils.run_async(_async("result"))
        res = utils.run_async(_async("result"))
        res = utils.run_async(_async("result"))
        print("ppp " + res)
    except Exception as exp:
        print(str(exp))

    pass
_sync()

async def test1():

    _sync()
    print("here")

    # await app.initialize()

    # agent = ConversationalChatAgent()
    # await agent.run(ctx=None)
    # nc = await bus.client()
    # await nc.drain()


if __name__ == '__main__':
    loop = asyncio.get_event_loop()
    loop.run_until_complete(unittest.main())
    try:
        loop.run_forever()
    finally:
        loop.close()
