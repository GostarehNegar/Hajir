from fastapi import FastAPI
import os
import logging
import brainpy
from brainpy import Knowledge, MsgContext, bus, utils, config, constants

from brainpy.models import BrainModels
from brainpy.agent.agentcontext import AgentContext
from brainpy.agent import get_agent
import brainpy.api

logger = logging.getLogger()
_app = FastAPI()
class App():
    def __init__(self):
        self.name = "lll"
        self.knowledge = Knowledge()
        self.logger = logging.getLogger()
        self.api = _app
        pass
    async def foo_handler(self, msg: MsgContext):
        print(msg.msg.subject)
        ttt = msg.GetPayload(BrainModels.BrainRequestModel)

        pass

    async def do_make_reply(self, msg: BrainModels.BrainRequestModel):
        print("-----------------------------")
        # self.logger.info(msg.msg.subject)
        try:
            data = msg
            ctx = AgentContext(data, kb=self.knowledge)
            agent = get_agent("")
            await agent.run(ctx)
            return ctx.reply
            await msg.Reply(ctx.reply)
        except Exception as exp:
            ctx.creat_reply_from_error(exp)
            return ctx.reply
            await msg.Reply(ctx.reply)
            print("$$ Error: ", exp)
            pass

    async def _make_reply(self, msg: MsgContext):
        print(msg.msg.subject)
        print("-----------------------------")
        # self.logger.info(msg.msg.subject)
        try:
            data = msg.GetPayload(BrainModels.BrainRequestModel)
            ctx = AgentContext(data, kb=self.knowledge)
            agent = get_agent("")
            await agent.run(ctx)
            await msg.Reply(ctx.reply)
        except Exception as exp:
            ctx.creat_reply_from_error(exp)
            await msg.Reply(ctx.reply)
            print("$$ Error: ", exp)
            pass

    async def initialize(self):
        logger.info("Initializing. app:'{}'".format(config.node_name))
        os.environ["OPENAI_API_KEY"] = utils.get_openai_key()
        # self.bus = bus
        # await bus.initialize(config.bus)
        # await bus.subscribe(constants.subjects.foo, self.foo_handler)
        # await bus.subscribe(constants.subjects.make_reply,self._make_reply)
        brainpy.api.initialize(self.api)
        await self.knowledge.initialize()
        logger.info("Successfully Initialized.")


app = App()

@app.api.post("/makereply")
async def Insert(c: BrainModels.BrainRequestModel):
    print(c.Input)
    return await app.do_make_reply(c)

@app.api.get("/exec/")
async def command(cmd: str = ""):
        
    f = cmd
    return cmd.capitalize()

    pass
