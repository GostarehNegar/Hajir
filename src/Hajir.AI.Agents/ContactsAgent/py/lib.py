"""
    Version 0.0.3
        utils
    Version 0.0.2
        * agent health 
    Version 0.0.1
"""
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
from pydantic import BaseModel, TypeAdapter
import random
import logging
from models import AgentRequest, AgentResponse
import models

from cachetools import TTLCache, LRUCache
from enum import Enum
import time

executor = concurrent.futures.ThreadPoolExecutor(
    max_workers=5,
)
background_tasks = set()


class CancellationToken():
    def __init__(self):
        self.IsCanceled = False
        pass

    def Cancel(self):
        self.IsCanceled = True


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
            self.settings.node_name, random.randint(10000, 99999))
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

    async def request(self, subject: str, payload: any, timeout=30):
        client = await self._client()
        header = {"from": self.name}
        str = payload.json() if isinstance(payload, BaseModel) else json.dumps(payload)
        # payload = json.dumps(data).encode()

        ret = await client.request(subject=subject, payload=str.encode(), headers=header, timeout=timeout)
        return MsgContext(ret, self)

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
            adapter = TypeAdapter(t)
            return adapter.validate_json(self.msg.data.decode())
            return json.loads(self.msg.data.decode()) if t == None else pydantic.parse_raw_as(t, txt)
        except:
            pass
        try:
            return json.loads(self.msg.data.decode())
        except:
            pass
        return self.msg.data.decode()


bus = Bus()


class CancellationToken():
    def __init__(self):
        self.IsCancelled = False
        pass

    def Cancel(self):
        self.IsCancelled = True


class BaseAgent():
    def __init__(self, name: str, description: str):
        self.name = name
        self.description = description
        self.stopped = False
        self.cancellationToken = CancellationToken()
        self.logger = logging.getLogger(self.name)

        pass

    async def get_llm_params(self) -> models.LLMParameters:
        res: MsgContext = await self.bus.request(Subjects.AI.Agents.Mamagements.get_available_llms, {})
        repl = res.GetPayload(models.GetAvailableLLMsResponse)
        return repl.Default

    async def start(self, bus: Bus, cancellationToken: CancellationToken = None):
        self.bus = bus
        if (cancellationToken != None):
            self.cancellationToken = cancellationToken
        await bus.subscribe(Subjects.AI.Agents.agent_request(self.name), self.handler)
        self.logger.info(f"Agent {self.name} started.")
        asyncio.create_task(self.heartbeat())
        pass

    async def stop(self):
        self.stopped = True

    async def heartbeat(self):
        while not self.cancellationToken.IsCancelled:
            # self.logger.info("heartbeat")
            await bus.publish(Subjects.AI.Agents.Mamagements.heartbeat, {
                "Name": self.name,
                "Description": self.description
            })
            await asyncio.sleep(6)

        pass

    async def handler(self, msg: MsgContext):
        repl = await self.reply(msg.GetPayload(AgentRequest))
        await msg.Reply(repl)

        pass

    async def reply(self, req: AgentRequest) -> AgentResponse:
        pass

    async def stop():
        pass


class BaseTool():
    def __init__(self, metadata: models.ToolMetadata = None):
        self.metadata: models.ToolMetadata = metadata
        if self.metadata == None:
            self.metadata = models.ToolMetadata(
                "", "", [], {"type": "object", "description": "Statistics about the text"})

    async def _handle(ctx: MsgContext):
        msg = ctx.GetPayload({})
        pass

    async def initialize(self):
        await bus.subscribe(self.metadata.subject, self._handle)

    async def run(request: models.ToolRequest, context: models.SessionContext):
        return None


class Subjects():
    pass


class Subjects():

    ai = "ai"

    class AI():

        class Agents():
            class Mamagements():
                pass
                _perfix = "ai.agents.management"
                heartbeat = _perfix + ".heartbeat"
                recruitsquad = _perfix+".squad_recruit"
                listagents = _perfix+".list"
                listools = _perfix+".listtools"
                new_conversation = "new_conversation"
                get_available_llms = _perfix+".getllms"
            request = "ai.agents.request"

            def agent_request(agent: str) -> str:
                return f"ai.agents.request.{agent}"
            
            def health_request_subject(agent:str)->str:
                return f"health.agents.request.{agent}"
            
            captainsquad = "captain"


            class PMOAssistant():
                _perfix = 'ai.agents.pmoassistant'
                list_activities = _perfix+".listactivities"
                my_activities = _perfix+".myactivities"
                timesheet = _perfix+".timesheet"
                get_desc = _perfix+".getdescription"
                set_desc = _perfix+".setdescription"

    pass


class Cache_Dep:
    def __init__(self):
        self._caches: Dict[int, LRUCache] = {}
        self._ttl_caches: Dict[int, TTLCache] = {}

        pass
    # Generic type variable
    T = TypeVar('T')

    # Dictionary to hold multiple caches with different sizes
    def get_cached(self,
                   key: str,
                   value_provider: Callable[[], T],
                   maxsize: int = 128
                   ) -> T:
        """
        Get a cached value or compute it using the provider function.
        Uses LRU (Least Recently Used) eviction strategy.

        Args:
            key: Cache key (string)
            value_provider: Function that provides the value if not in cache
            maxsize: Maximum number of items in cache (default: 128)

        Returns:
            The cached or computed value of type T

        Example:
            def fetch_user():
                return {"id": 1, "name": "John"}

            user = get_cached("user:1", fetch_user, maxsize=100)
        """
        # Get or create cache for this maxsize
        if maxsize not in self._caches:
            self._caches[maxsize] = LRUCache(maxsize=maxsize)

        cache = self._caches[maxsize]

        if key in cache:
            return cache[key]

        # Compute value using provider
        value = value_provider()

        # Store in cache (will evict LRU item if full)
        cache[key] = value

        return value

    def clear_cache(self, maxsize: int = None):
        """
        Clear cached values.

        Args:
            maxsize: If specified, clear only cache with this maxsize.
                    If None, clear all caches.
        """
        if maxsize is not None:
            if maxsize in self._caches:
                self._caches[maxsize].clear()
        else:
            for cache in self._caches.values():
                cache.clear()

    def get_cache_info(self, maxsize: int = None):
        """
        Get cache statistics.

        Args:
            maxsize: If specified, get info for cache with this maxsize.
                    If None, get info for all caches.
        """
        if maxsize is not None:
            if maxsize in self._caches:
                cache = self._caches[maxsize]
                return {
                    "maxsize": maxsize,
                    "current_size": len(cache),
                    "keys": list(cache.keys())
                }
            return None

        return {
            size: {
                "current_size": len(cache),
                "keys": list(cache.keys())
            }
            for size, cache in self._caches.items()
        }

    def get_ttl_cached(self,
                       key: str,
                       value_provider: Callable[[], T],
                       ttl_seconds: int = 300
                       ) -> T:
        """
        Get a cached value or compute it using the provider function.

        Args:
            key: Cache key (string)
            value_provider: Function that provides the value if not in cache
            ttl_seconds: Time to live in seconds (default: 300 = 5 minutes)

        Returns:
            The cached or computed value of type T

        Example:
            def fetch_user():
                return {"id": 1, "name": "John"}

            user = get_cached("user:1", fetch_user, ttl_seconds=600)
        """
        # Get or create cache for this TTL
        if ttl_seconds not in self._ttl_caches:
            self._ttl_caches_caches[ttl_seconds] = TTLCache(
                maxsize=1000, ttl=ttl_seconds)

        cache = self._ttl_caches[ttl_seconds]

        if key in cache:
            return cache[key]

        # Compute value using provider
        value = value_provider()

        # Store in cache
        cache[key] = value

        return value


def clear_ttl_cache(self, ttl_seconds: int = None):
    """
    Clear cached values.

    Args:
        ttl_seconds: If specified, clear only cache with this TTL.
                    If None, clear all caches.
    """
    if ttl_seconds is not None:
        if ttl_seconds in self._ttl_caches:
            self._ttl_caches[ttl_seconds].clear()
    else:
        for cache in self._ttl_caches.values():
            cache.clear()


def get_cache_info(self, ttl_seconds: int = None):
    """
    Get cache statistics.

    Args:
        ttl_seconds: If specified, get info for cache with this TTL.
                    If None, get info for all caches.
    """
    if ttl_seconds is not None:
        if ttl_seconds in self._ttl_caches:
            cache = self._ttl_caches[ttl_seconds]
            return {
                "ttl": ttl_seconds,
                "size": len(cache),
                "maxsize": cache.maxsize
            }
        return None

    return {
        ttl: {
            "size": len(cache),
            "maxsize": cache.maxsize
        }
        for ttl, cache in self._ttl_caches.items()
    }


class SlidingTTLCache:
    """Cache with sliding TTL - resets expiration on each access"""

    def __init__(self, maxsize: int, ttl: int):
        self.maxsize = maxsize
        self.ttl = ttl
        # key -> (value, last_access_time)
        self._cache: Dict[str, Tuple[any, float]] = {}

    def __contains__(self, key: str) -> bool:
        if key not in self._cache:
            return False

        value, last_access = self._cache[key]

        # Check if expired
        if time.time() - last_access > self.ttl:
            del self._cache[key]
            return False

        return True

    def __getitem__(self, key: str):
        if key not in self:
            raise KeyError(key)

        value, _ = self._cache[key]
        # Update last access time (sliding window)
        self._cache[key] = (value, time.time())
        return value

    def __setitem__(self, key: str, value):
        # Enforce maxsize
        if key not in self._cache and len(self._cache) >= self.maxsize:
            # Remove oldest (by last access time)
            oldest_key = min(self._cache.items(), key=lambda x: x[1][1])[0]
            del self._cache[oldest_key]

        self._cache[key] = (value, time.time())

    def clear(self):
        self._cache.clear()

    def keys(self):
        # Clean up expired keys first
        current_time = time.time()
        expired = [k for k, (v, t) in self._cache.items()
                   if current_time - t > self.ttl]
        for k in expired:
            del self._cache[k]
        return list(self._cache.keys())

    def __len__(self):
        # Clean up expired entries
        current_time = time.time()
        expired = [k for k, (v, t) in self._cache.items()
                   if current_time - t > self.ttl]
        for k in expired:
            del self._cache[k]
        return len(self._cache)


# Generic type variable
T = TypeVar('T')


class CacheStrategy(Enum):
    """Cache eviction strategy"""
    LRU = "lru"
    TTL = "ttl"              # Absolute TTL - expires after X seconds from creation
    SLIDING_TTL = "sliding"  # Sliding TTL - expires after X seconds from last access
    HYBRID = "hybrid"        # LRU + absolute TTL


class Cache:
    """
    Generic cache with support for LRU, TTL, and Hybrid strategies.
    Strategy is specified per-get operation for maximum flexibility.

    Examples:
        cache = Cache()

        # LRU cache
        user = cache.get("user:1", fetch_user, strategy=CacheStrategy.LRU, maxsize=100)

        # TTL cache
        prices = cache.get("prices", fetch_prices, strategy=CacheStrategy.TTL, ttl_seconds=60)

        # Hybrid cache
        data = cache.get("data:1", fetch_data, strategy=CacheStrategy.HYBRID, 
                        maxsize=100, ttl_seconds=300)
    """

    def __init__(self):
        """Initialize cache manager with storage for different cache types."""
        self._lru_caches: Dict[int, LRUCache] = {}
        self._ttl_caches: Dict[int, TTLCache] = {}
        self._sliding_ttl_caches: Dict[int, SlidingTTLCache] = {}
        self._hybrid_caches: Dict[Tuple[int, int], TTLCache] = {}
        self._locks: Dict[str, asyncio.Lock] = {}  # Locks for async operations

    def get(
        self,
        key: str,
        value_provider: Optional[Callable[[], T]] = None,
        strategy: CacheStrategy = CacheStrategy.SLIDING_TTL,
        maxsize: int = 128,
        ttl_seconds: Optional[int] = 300
    ) -> Optional[T]:
        """
        Get a cached value or compute it using the provider function.

        Args:
            key: Cache key (string)
            value_provider: Function that provides the value if not in cache (optional)
                           If None, returns None if key not found
            strategy: Cache eviction strategy (LRU, TTL, SLIDING_TTL, or HYBRID)
            maxsize: Maximum number of items in cache (default: 128)
            ttl_seconds: Time to live in seconds (required for TTL, SLIDING_TTL, and HYBRID strategies)

        Returns:
            The cached or computed value of type T, or None if not found and no provider given

        Strategy Details:
            - LRU: Evicts least recently used when full
            - TTL: Expires after X seconds from creation (absolute)
            - SLIDING_TTL: Expires after X seconds from last access (resets on read)
            - HYBRID: LRU + absolute TTL
        """
        if strategy == CacheStrategy.LRU:
            return self._get_lru(key, value_provider, maxsize)
        elif strategy == CacheStrategy.TTL:
            if ttl_seconds is None:
                raise ValueError("ttl_seconds is required for TTL strategy")
            return self._get_ttl(key, value_provider, ttl_seconds, maxsize)
        # elif strategy == CacheStrategy.SLIDING_TTL:
        #     if ttl_seconds is None:
        #         raise ValueError(
        #             "ttl_seconds is required for SLIDING_TTL strategy")
        #     return self._get_sliding_ttl(key, value_provider, ttl_seconds, maxsize)
        elif strategy == CacheStrategy.HYBRID:
            if ttl_seconds is None:
                raise ValueError("ttl_seconds is required for HYBRID strategy")
            return self._get_hybrid(key, value_provider, maxsize, ttl_seconds)
        else:
            raise ValueError(f"Unknown strategy: {strategy}")

    def _get_lru(self, key: str, value_provider: Optional[Callable[[], T]], maxsize: int) -> Optional[T]:
        """Get from LRU cache"""
        if maxsize not in self._lru_caches:
            self._lru_caches[maxsize] = LRUCache(maxsize=maxsize)

        cache = self._lru_caches[maxsize]

        if key in cache:
            return cache[key]

        # If no provider, return None
        if value_provider is None:
            return None

        value = value_provider()
        cache[key] = value
        return value

    def _get_ttl(self, key: str, value_provider: Optional[Callable[[], T]], ttl_seconds: int, maxsize: int) -> Optional[T]:
        """Get from TTL cache"""
        if ttl_seconds not in self._ttl_caches:
            self._ttl_caches[ttl_seconds] = TTLCache(
                maxsize=maxsize, ttl=ttl_seconds)

        cache = self._ttl_caches[ttl_seconds]

        if key in cache:
            return cache[key]

        # If no provider, return None
        if value_provider is None:
            return None

        value = value_provider()
        cache[key] = value
        return value

    def _get_hybrid(self, key: str, value_provider: Optional[Callable[[], T]], maxsize: int, ttl_seconds: int) -> Optional[T]:
        """Get from hybrid cache (LRU + TTL)"""
        cache_key = (maxsize, ttl_seconds)

        if cache_key not in self._hybrid_caches:
            self._hybrid_caches[cache_key] = TTLCache(
                maxsize=maxsize, ttl=ttl_seconds)

        cache = self._hybrid_caches[cache_key]

        if key in cache:
            return cache[key]

        # If no provider, return None
        if value_provider is None:
            return None

        value = value_provider()
        cache[key] = value
        return value

    def clear(self, strategy: Optional[CacheStrategy] = None):
        """
        Clear cached values.

        Args:
            strategy: If specified, clear only caches of this type.
                     If None, clear all caches.
        """
        if strategy is None or strategy == CacheStrategy.LRU:
            for cache in self._lru_caches.values():
                cache.clear()

        if strategy is None or strategy == CacheStrategy.TTL:
            for cache in self._ttl_caches.values():
                cache.clear()

        if strategy is None or strategy == CacheStrategy.SLIDING_TTL:
            for cache in self._sliding_ttl_caches.values():
                cache.clear()

        if strategy is None or strategy == CacheStrategy.HYBRID:
            for cache in self._hybrid_caches.values():
                cache.clear()

    def info(self, strategy: Optional[CacheStrategy] = None) -> dict:
        """
        Get cache statistics.

        Args:
            strategy: If specified, get info for caches of this type.
                     If None, get info for all caches.

        Returns:
            Dictionary with cache information
        """
        info = {}

        if strategy is None or strategy == CacheStrategy.LRU:
            info["LRU"] = {
                maxsize: {
                    "current_size": len(cache),
                    "maxsize": maxsize,
                    "keys": list(cache.keys())
                }
                for maxsize, cache in self._lru_caches.items()
            }

        if strategy is None or strategy == CacheStrategy.TTL:
            info["TTL"] = {
                ttl: {
                    "current_size": len(cache),
                    "maxsize": cache.maxsize,
                    "ttl": ttl,
                    "keys": list(cache.keys())
                }
                for ttl, cache in self._ttl_caches.items()
            }

        if strategy is None or strategy == CacheStrategy.SLIDING_TTL:
            info["SLIDING_TTL"] = {
                ttl: {
                    "current_size": len(cache),
                    "maxsize": cache.maxsize,
                    "ttl": ttl,
                    "keys": cache.keys()
                }
                for ttl, cache in self._sliding_ttl_caches.items()
            }

        if strategy is None or strategy == CacheStrategy.HYBRID:
            info["HYBRID"] = {
                f"maxsize={ms},ttl={ttl}": {
                    "current_size": len(cache),
                    "maxsize": ms,
                    "ttl": ttl,
                    "keys": list(cache.keys())
                }
                for (ms, ttl), cache in self._hybrid_caches.items()
            }

        return info

    def __repr__(self) -> str:
        """String representation of cache manager"""
        lru_count = len(self._lru_caches)
        ttl_count = len(self._ttl_caches)
        sliding_count = len(self._sliding_ttl_caches)
        hybrid_count = len(self._hybrid_caches)
        return f"Cache(LRU: {lru_count}, TTL: {ttl_count}, SLIDING_TTL: {sliding_count}, HYBRID: {hybrid_count})"


cache = Cache()


    
