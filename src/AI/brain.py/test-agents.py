import asyncio
from brainpy.agent.AssistantAgent import AsssitantConversationalChatAgent
from brainpy.agent.agentcontext import AgentContext
from brainpy.models.BrainModels import BrainRequestModel
from brainpy.knowledge import Knowledge
import datetime

async def main1():
    # first we need a request model
    # datetime.datetime.fromisoformat("2023-09-20T05:00:00")
    request = BrainRequestModel()
    request.Input = "whats the time offset beteen tomorrow 5 Am and now."

    # then we need a knowledge base!
    kb = Knowledge()
    await kb.initialize()


    # to run an agent we need to instantate the agent
    agent = AsssitantConversationalChatAgent()
    # then we need a context
    context = AgentContext(request, kb)

    result = await agent.run(context)

    print(result.reply.Error)
    

    pass

async def main2():
     # first we need a request model
    # datetime.datetime.fromisoformat("2023-09-20T05:00:00")
    request = BrainRequestModel()
    request.Input = "please schedule a meeting with John and Marry for tomorrow at 6 PM."

    # then we need a knowledge base!
    kb = Knowledge()
    await kb.initialize()


    # to run an agent we need to instantate the agent
    agent = AsssitantConversationalChatAgent()
    # then we need a context
    context = AgentContext(request, kb)

    result = await agent.run(context)

    print(result.reply.Error)
    

if __name__ == '__main__':
    loop = asyncio.get_event_loop()
    loop.run_until_complete(main1())
    try:
        loop.run_forever()
    finally:
        loop.close()
