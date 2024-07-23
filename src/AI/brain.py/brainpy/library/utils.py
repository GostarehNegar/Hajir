import logging

from typing import TYPE_CHECKING, Any, Coroutine, TypeVar
from threading import Thread
import asyncio
import random
import concurrent.futures
executor = concurrent.futures.ThreadPoolExecutor(max_workers=5)

class Utils:
    def getLogger():
        return logging.getLogger()

    logger = logging.getLogger()

    def serialize_chain_answer(answer):
        pass

    def deserialize_make_repy_request(req):
        pass

    def get_openai_key():
        
    
        try:

            with open('.env') as f:
                lines = f.readlines()
                return lines[0].replace("OPENAI_API_KEY", "").strip().replace("=", "").strip()
        except:
            return None

    def is_null_or_empty(s: str):
        return s == None or s.isspace()

    def random_name(base: str):
        
        return base+"_"+str(random.randint(10000, 99999))
        pass

    T = TypeVar("T")

    def run_async(coro: Coroutine[Any, Any, T], timeout=10) -> T:
        def run():
            loop = asyncio.new_event_loop()
            res = loop.run_until_complete(coro)
            loop.close()
            return res
        future = executor.submit(run)
        if  future.exception():
            raise(future.exception())
        return future.result()


    def run_async_deprecated(coro: Coroutine[Any, Any, T], timeout=10) -> T:
        res = None
        exp = None

        def run():
            nonlocal res
            nonlocal exp
            try:
                loop = asyncio.new_event_loop()
                res = loop.run_until_complete(coro)
                loop.close()
            except Exception as _exp:
                exp = _exp
                pass
        t = Thread(target=run)
        t.start()
        t.join(timeout=timeout)
        if t.is_alive():
            t = TimeoutError()
            t.strerror = "Timeout"
            raise (t)
        if not exp == None:
            raise (exp)
        return res

        pass


utils = Utils
