from langchain.agents import AgentExecutor, create_tool_calling_agent
from langchain.tools import tool, StructuredTool
from langchain_core.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain_core.messages import HumanMessage, AIMessage
from langchain_openai import ChatOpenAI
from pydantic import BaseModel, Field
import math
from lib import bus, CancellationToken, MsgContext, Subjects,BaseAgent
import models
import logging
import asyncio
logger = logging.getLogger(__name__)
# Define sample tools using the @tool decorator
class pl:
    pricelist:models.ProductSearchResult = None
@tool
async def calculator(expression: str) -> str:
    """Evaluates a mathematical expression. 
    Use this for any math calculations.
    
    Args:
        expression: A mathematical expression as a string (e.g., "2 + 2", "sqrt(16)")
    """
    await asyncio.sleep(1)
    try:
        # Use math functions if needed
        result = eval(expression, {"__builtins__": {}}, {
            "sqrt": math.sqrt,
            "pow": math.pow,
            "sin": math.sin,
            "cos": math.cos,
            "pi": math.pi,
        })
        return str(result)
    except Exception as e:
        return f"Error calculating: {str(e)}"


@tool
def word_counter(text: str) -> str:
    """Counts the number of words in a given text.
    
    Args:
        text: The text to count words in
    """
    word_count = len(text.split())
    return f"The text contains {word_count} words."


@tool
def text_reverser(text: str) -> str:
    """Reverses the given text.
    
    Args:
        text: The text to reverse
    """
    return text[::-1]


@tool
def temperature_converter(temperature: float, from_unit: str, to_unit: str) -> str:
    """Converts temperature between Celsius, Fahrenheit, and Kelvin.
    
    Args:
        temperature: The temperature value to convert
        from_unit: The unit to convert from ('C', 'F', or 'K')
        to_unit: The unit to convert to ('C', 'F', or 'K')
    """
    # Convert to Celsius first
    if from_unit == 'F':
        celsius = (temperature - 32) * 5/9
    elif from_unit == 'K':
        celsius = temperature - 273.15
    else:
        celsius = temperature
    
    # Convert from Celsius to target unit
    if to_unit == 'F':
        result = celsius * 9/5 + 32
    elif to_unit == 'K':
        result = celsius + 273.15
    else:
        result = celsius
    
    return f"{temperature}°{from_unit} = {result:.2f}°{to_unit}"


# ===== Dynamic Tool Creation Without Decorator =====

# Method 1: Using StructuredTool.from_function
def get_string_length(text: str) -> str:
    """Regular Python function to get string length."""
    length = len(text)
    return f"The string has {length} characters."

# Create tool dynamically
string_length_tool = StructuredTool.from_function(
    func=get_string_length,
    name="string_length",
    description="Returns the length of a string (number of characters including spaces)"
)


# Method 2: Using StructuredTool with Pydantic schema
class PalindromeInput(BaseModel):
    """Input schema for palindrome checker."""
    text: str = Field(description="The text to check if it's a palindrome")

def check_palindrome(text: str) -> str:
    """Check if a text is a palindrome."""
    cleaned = text.lower().replace(" ", "").replace(",", "").replace(".", "")
    is_palindrome = cleaned == cleaned[::-1]
    return f"'{text}' is {'a palindrome' if is_palindrome else 'not a palindrome'}."

# Create tool with explicit schema
palindrome_tool = StructuredTool(
    name="palindrome_checker",
    description="Checks if a given text is a palindrome (reads the same forwards and backwards)",
    func=check_palindrome,
    args_schema=PalindromeInput
)


# Method 3: Creating a tool from a lambda (simple cases)
def create_list_tool(items: str) -> str:
    """Split comma-separated items and count them."""
    item_list = [item.strip() for item in items.split(",")]
    return f"Found {len(item_list)} items: {', '.join(item_list)}"

list_counter_tool = StructuredTool.from_function(
    func=create_list_tool,
    name="list_counter",
    description="Takes a comma-separated list of items and counts how many items there are"
)
async def search_products(name: str) -> models.ProductSearchResult:
    """Search for products by searching phrase in product names."""
    # Mock data - in real implementation, this would query a database
    await asyncio.sleep(1)
    pricelist=pl.pricelist
    if pricelist ==None:
        return models.ProductSearchResult()
    

    ret1 = models.ProductSearchResult()
    ret1.products = list(filter(lambda x: name.lower() in x.name.lower(), pricelist.products))[:10]
    ret1.total_found = len(ret1.products)
    return ret1

product_search_tool = StructuredTool.from_function(
    #func=  search_products,
    coroutine=search_products,
    name="search_products",
    description="Search for products and prices by name. Returns product listings with price information.",
    #args_schema=models.ProductSearchInput,
    return_direct=False
)


# Initialize the LLM
url ="https://api.deepseek.com"
api_key = "sk-fec93ba732c046b38b35263b0a4c004d" #"sk-3b8842c4b8de41b48ad350662886e849"
GPT_OSS ="deepseek-chat"
#llm = ChatOpenAI(model="gpt-4", temperature=0)
llm = ChatOpenAI(model=GPT_OSS, temperature=0, base_url=url,api_key=api_key)
# Gather all tools (decorator-based and dynamically created)
tools = [
    calculator, 
    #word_counter, 
    #text_reverser, 
    #temperature_converter,
    #string_length_tool,      # Dynamic tool
    #palindrome_tool,         # Dynamic tool with schema
    #list_counter_tool        # Dynamic tool from function
    product_search_tool
]

# Create the prompt template with chat history support
prompt = ChatPromptTemplate.from_messages([
    ("system", 
     """
     You are a helpful assistant with access to various tools. Use them when needed to answer questions accurately. 
     Always use search_products tool when you are asked to search for products or price.
     DO NOT TRY TO LIST PRODUCTS BY YOURSELF ALWAYS USE TOOL.
     Examples:
     1. To list Batteries use search_products('باتری')
  
     """),
    MessagesPlaceholder(variable_name="chat_history"),
    ("human", "{input}"),
    ("placeholder", "{agent_scratchpad}"),
])

# Create the agent
agent = create_tool_calling_agent(llm, tools, prompt)

# Create the agent executor
agent_executor = AgentExecutor(
    agent=agent,
    tools=tools,
    verbose=True,
    handle_parsing_errors=True
)

class PriceAgent(BaseAgent):

    def __init__(self, ):
        
        super().__init__("Reza",
                         'Sale agent that knows about products and prices.')
        
    def ToMessage(self, msg:models.ChatMessage):
        return HumanMessage(msg.content[0].get('text')) if msg.role=='user' else AIMessage(msg.content[0].get('text'))

    async def get_price_loop(self):
        while not self.cancellationToken.IsCancelled:
           
            #self.logger.info("heartbeat")
           repl = await bus.request('pricelist',{'name':'name'})
           pl.pricelist= repl.GetPayload(models.ProductSearchResult)
           await asyncio.sleep(60)

    async def init(self ):
        asyncio.create_task(self.get_price_loop())
        pass

    async def reply(self, req: models.AgentRequest):
        logger.info(f"Smith starts.")
        _history = [self.ToMessage(msg)
                    for msg in req.chat_history]
        response = await agent_executor.ainvoke({
            "input": req.input_text,
            "chat_history": _history
            })
        
        return models.AgentResponse(text=response['output'])
reza = PriceAgent()



class SamplePriceTool():
    pass
    async def run(self, cancel: CancellationToken = CancellationToken()):
        # get agents
        logger.info("START")
        await bus.subscribe(Subjects.AI.Agents.agent_request("reza"), self.handle)
        pass
    async def handle(self,msg:MsgContext):
        payload = msg.GetPayload(models.AgentRequest)
        response = agent_executor.invoke({
        "input": payload.input_text,
        "chat_history": payload.chat_history
        })
        return models.AgentResponse(text=response['output'])



# Example usage
if __name__ == "__main__":
    # Initialize chat history
    chat_history = []
    
    # Example 1: Using the calculator
    print("=" * 60)
    print("Example 1: Math calculation")
    print("=" * 60)
    user_input = "What is the square root of 144 plus 5 times 3?"
    response = agent_executor.invoke({
        "input": user_input,
        "chat_history": chat_history
    })
    print(f"\nFinal Answer: {response['output']}\n")
    
    # Update chat history
    chat_history.append(HumanMessage(content=user_input))
    chat_history.append(AIMessage(content=response['output']))
    
    # Example 2: Follow-up question (uses previous context)
    print("=" * 60)
    print("Example 2: Follow-up question with context")
    print("=" * 60)
    user_input = "Can you multiply that result by 2?"
    response = agent_executor.invoke({
        "input": user_input,
        "chat_history": chat_history
    })
    print(f"\nFinal Answer: {response['output']}\n")
    
    # Update chat history
    chat_history.append(HumanMessage(content=user_input))
    chat_history.append(AIMessage(content=response['output']))
    
    # Example 3: Using word counter
    print("=" * 60)
    print("Example 3: Word counting")
    print("=" * 60)
    user_input = "How many words are in this sentence: 'The quick brown fox jumps over the lazy dog'?"
    response = agent_executor.invoke({
        "input": user_input,
        "chat_history": chat_history
    })
    print(f"\nFinal Answer: {response['output']}\n")
    
    # Update chat history
    chat_history.append(HumanMessage(content=user_input))
    chat_history.append(AIMessage(content=response['output']))
    
    # Example 4: Follow-up referencing previous question
    print("=" * 60)
    print("Example 4: Reference to previous conversation")
    print("=" * 60)
    user_input = "Can you reverse that sentence?"
    response = agent_executor.invoke({
        "input": user_input,
        "chat_history": chat_history
    })
    print(f"\nFinal Answer: {response['output']}\n")
    
    # Update chat history
    chat_history.append(HumanMessage(content=user_input))
    chat_history.append(AIMessage(content=response['output']))
    
    # Example 5: Using dynamically created tools
    print("=" * 60)
    print("Example 5: Using dynamic string length tool")
    print("=" * 60)
    user_input = "What is the length of 'Hello, World!'?"
    response = agent_executor.invoke({
        "input": user_input,
        "chat_history": chat_history
    })
    print(f"\nFinal Answer: {response['output']}\n")
    
    # Update chat history
    chat_history.append(HumanMessage(content=user_input))
    chat_history.append(AIMessage(content=response['output']))
    
    # Example 6: Using palindrome checker
    print("=" * 60)
    print("Example 6: Palindrome checker")
    print("=" * 60)
    user_input = "Is 'A man a plan a canal Panama' a palindrome?"
    response = agent_executor.invoke({
        "input": user_input,
        "chat_history": chat_history
    })
    print(f"\nFinal Answer: {response['output']}\n")
    
    # Update chat history
    chat_history.append(HumanMessage(content=user_input))
    chat_history.append(AIMessage(content=response['output']))
    
    # Example 7: Using list counter
    print("=" * 60)
    print("Example 7: List counter")
    print("=" * 60)
    user_input = "Count the items in this list: apple, banana, orange, grape, mango"
    response = agent_executor.invoke({
        "input": user_input,
        "chat_history": chat_history
    })
    print(f"\nFinal Answer: {response['output']}\n")
    
    # Update chat history
    chat_history.append(HumanMessage(content=user_input))
    chat_history.append(AIMessage(content=response['output']))
    
    # Example 8: Interactive chat loop (commented out by default)
    """
    print("=" * 60)
    print("Interactive Chat (type 'quit' to exit)")
    print("=" * 60)
    
    while True:
        user_input = input("\nYou: ").strip()
        if user_input.lower() in ['quit', 'exit', 'bye']:
            print("Goodbye!")
            break
        
        if not user_input:
            continue
        
        response = agent_executor.invoke({
            "input": user_input,
            "chat_history": chat_history
        })
        
        print(f"\nAssistant: {response['output']}")
        
        # Update chat history
        chat_history.append(HumanMessage(content=user_input))
        chat_history.append(AIMessage(content=response['output']))
    """