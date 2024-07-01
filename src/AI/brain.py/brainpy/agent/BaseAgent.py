from brainpy.agent.agentcontext import AgentContext
from langchain.agents.tools import Tool
from typing import List


class BaseAgent:
    def __init__(self, name: str) -> None:
        self.name = name
        pass

    async def run(self, ctx: AgentContext, tools: List[Tool] = []):
        try:
            response = await self._run(ctx, tools=tools)
        except Exception as exp:
            response = ctx.creat_reply_from_error(exp)
        return ctx
    async def _run(self, ctx: AgentContext, tools: List[Tool] = []):
        pass
