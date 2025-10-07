from fastapi import FastAPI
from lib import bus, MsgContext, BaseAgent, CancellationToken, Subjects
from typing import List, Optional, Dict
import models
import logging

from openai import OpenAI
from tenacity import retry, stop_after_attempt, wait_exponential
from langchain_tools_sample import reza
server = FastAPI()
api_key = "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"
url = "https://openrouter.ai/api/v1"
logger = logging.getLogger(__name__)
GPT_OSS = "openai/gpt-oss-120b"

url ="https://api.deepseek.com"
api_key = "sk-fec93ba732c046b38b35263b0a4c004d" #"sk-3b8842c4b8de41b48ad350662886e849"
GPT_OSS ="deepseek-chat"

template = """Question: {question}
Answer: Let's think step by step."""




class App(FastAPI):

    async def run(self):
        
        await reza.start(bus)
        await reza.init()

        pass


app = App()
