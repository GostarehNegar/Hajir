import concurrent.futures
from typing import (
    Any,
    Awaitable,
    Callable,
    Tuple,
    Union,
    List,
    Optional,
    Dict,
    TypeVar,
    Type
)
import nats
from nats.aio.client import Client
from nats.aio.msg import Msg
import json
import asyncio
import pydantic
from pydantic import BaseModel
import random


executor = concurrent.futures.ThreadPoolExecutor(
    max_workers=5,
)
background_tasks = set()


class MsgContext:
    pass


class BusSettings(BaseModel):
    nats_connection_string: str = "nats://localhost:4222"
    node_name: str = "no_name"
    domain: str = "public"


class Bus():

    def __init__(self):
        self.name = "no_name"
        self.nc = None
        self.settings = BusSettings()

    async def _handle_mail_box(self, ctx: MsgContext):
        print(ctx.msg.subject)
        pass

    async def initialize(self, settings: BusSettings = None):
        self.settings = settings or BusSettings()
        self.name = "{}.{}".format(
            settings.node_name, random.randint(10000, 99999))
        await self.subscribe(self.name, cb=self._handle_mail_box)
        # logger.info("Bus successfully initialized. Name:'{}', Domian:'{}'".format(
        #     self.name, self.settings.domain))

    async def _client(self, refresh: bool = False) -> Client:
        if self.nc == None or refresh:
            self.nc = await nats.connect(self.settings.nats_connection_string)
        return self.nc

    def _extend(msg: Msg) -> MsgContext:
        return MsgContext(msg)

    async def subscribe(self, subject: str, cb: Optional[Callable[[MsgContext], Awaitable[None]]] = None,):
        """
            Subscribe to subject wit a call back to handle messages
            of that subject.
        """
        async def handler(msg: Msg):
            await cb(MsgContext(msg, self))
        client = await self._client()
        await client.subscribe(subject, cb=handler)
        return

        async def handler_2(msg: Msg):
            loop = asyncio.get_running_loop()
            with concurrent.futures.ThreadPoolExecutor() as pool:
                result = await loop.run_in_executor(pool, cb, MsgContext(msg, self))

        async def handler_3(msg: Msg):
            loop = asyncio.get_running_loop()
            # with concurrent.futures.ThreadPoolExecutor() as pool:
            result = await loop.run_in_executor(executor, cb, MsgContext(msg, self))
        task = asyncio.create_task(client.subscribe(subject, cb=handler_3))

        # Add task to the set. This creates a strong reference.
        background_tasks.add(task)
        # To prevent keeping references to finished tasks forever,
        # make each task remove its own reference from the set after
        # completion:
        task.add_done_callback(background_tasks.discard)

    async def publish(self, subject: str, payload: any):
        client = await self._client()
        header = {"from": self.name}
        str = payload.json() if isinstance(payload, BaseModel) else json.dumps(payload)
        # payload = json.dumps(data).encode()
        
        await client.publish(subject=subject, payload=str.encode(), headers=header)

    # async def initialize(self, name: str):
    #     self.nc = await nats.connect(config.nats_connection_string)
    #     pass


T = TypeVar('T')


class MsgContext():
    def __init__(self, msg: Msg, bus: Bus):
        self.msg = msg
        self.bus = bus

    async def Reply(self, repl: Any):
        # str = repl.json() if isinstance(repl, BaseModel) else json.dumps(repl)
        # str.encode()

        await self.bus.publish(self.msg.reply, repl)
        pass

    def GetData(self) -> Any:
        return json.loads(self.msg.data.decode())

    def GetPayload(self, t: Type[T] = None) -> T:
        txt = self.msg.data.decode()
        try:
            return json.loads(self.msg.data.decode()) if t == None else pydantic.parse_raw_as(t, txt)
        except:
            pass
        try:
            return json.loads(self.msg.data.decode())
        except:
            pass
        return self.msg.data.decode()


bus = Bus()
