"""
Dynamic LangChain Tools System using NATS

This module provides a system for dynamically discovering and using tools
implemented as NATS responders with LangChain agents.
"""

import json
import asyncio
from typing import Any, Dict, List, Optional, Type
from dataclasses import dataclass
from langchain.tools import Tool, StructuredTool
from langchain.agents import AgentExecutor, create_react_agent
from langchain.prompts import PromptTemplate
from langchain_core.language_models import BaseChatModel
from pydantic import BaseModel, Field, create_model

try:
    import nats
    from nats.aio.client import Client as NATS
except ImportError:
    raise ImportError("Please install nats-py: pip install nats-py")


@dataclass
class ToolParameter:
    """Represents a tool parameter with its metadata."""
    name: str
    type: str
    description: str
    required: bool = True
    default: Any = None


@dataclass
class ToolMetadata:
    """Metadata describing a NATS-based tool."""
    name: str
    description: str
    subject: str
    parameters: List[ToolParameter]
    returns: Dict[str, Any]


@dataclass
class ToolRequest:
    parameters: Dict[str, Any]
    


class NATSToolRegistry:
    """Registry for managing dynamic NATS-based tools."""

    def __init__(self, nats_url: str = "nats://localhost:4222"):
        self.nats_url = nats_url
        self.nc: Optional[NATS] = None
        self.tools: List[Tool] = []
        self.discovery_subject = "tools.discovery"
        self.metadata: List[ToolMetadata] = None

    async def connect(self):
        """Connect to NATS server."""
        self.nc = await nats.connect(self.nats_url)
        print(f"Connected to NATS at {self.nats_url}")

    async def disconnect(self):
        """Disconnect from NATS server."""
        if self.nc:
            await self.nc.close()
            print("Disconnected from NATS")

    async def discover_tools(self, timeout: float = 5.0) -> List[ToolMetadata]:
        """
        Discover available tools by sending a discovery request.

        Returns:
            List of ToolMetadata objects describing available tools
        """
        if not self.nc:
            raise RuntimeError("Not connected to NATS. Call connect() first.")

        try:
            # Send discovery request
            response = await self.nc.request(
                self.discovery_subject,
                b"",
                timeout=timeout
            )

            # Parse response
            tools_data = json.loads(response.data.decode())

            # Convert to ToolMetadata objects
            tools_metadata = []
            for tool_data in tools_data:
                parameters = [
                    ToolParameter(**param) for param in tool_data.get("parameters", [])
                ]
                metadata = ToolMetadata(
                    name=tool_data["name"],
                    description=tool_data["description"],
                    subject=tool_data["subject"],
                    parameters=parameters,
                    returns=tool_data.get("returns", {})
                )
                tools_metadata.append(metadata)

            print(f"Discovered {len(tools_metadata)} tools")
            return tools_metadata

        except asyncio.TimeoutError:
            print(f"Tool discovery timed out after {timeout} seconds")
            return []
        except Exception as e:
            print(f"Error during tool discovery: {e}")
            return []

    def _create_pydantic_schema(self, parameters: List[ToolParameter]) -> Type[BaseModel]:
        """Create a Pydantic model from tool parameters."""
        if not parameters:
            return None

        fields = {}

        for param in parameters:
            # Map type strings to Python types
            type_map = {
                "string": str,
                "str": str,
                "integer": int,
                "int": int,
                "float": float,
                "number": float,
                "boolean": bool,
                "bool": bool,
                "array": list,
                "list": list,
                "object": dict,
                "dict": dict,
            }

            param_type = type_map.get(param.type.lower(), str)

            # Handle optional parameters
            if not param.required:
                param_type = Optional[param_type]
                default_value = param.default if param.default is not None else None
            else:
                default_value = ...

            fields[param.name] = (
                param_type,
                Field(default=default_value, description=param.description)
            )

        # Create dynamic Pydantic model
        return create_model("ToolInput", **fields)

    def _parse_tool_input(self, input_str: str, parameters: List[ToolParameter]) -> Dict[str, Any]:
        """
        Parse the tool input string into a parameter dictionary.
        Handles both JSON format and key-value format.
        """
        input_str = input_str.strip()

        # Try to parse as JSON first
        try:
            params = json.loads(input_str)
            if isinstance(params, dict):
                return params
        except (json.JSONDecodeError, ValueError):
            pass

        # Try comma-separated key=value format
        params = {}
        try:
            # Split by comma and parse key=value pairs
            pairs = [p.strip() for p in input_str.split(',')]
            for pair in pairs:
                if '=' in pair:
                    key, value = pair.split('=', 1)
                    key = key.strip()
                    value = value.strip().strip('"\'')

                    # Try to convert to appropriate type
                    param_meta = next(
                        (p for p in parameters if p.name == key), None)
                    if param_meta:
                        if param_meta.type in ['int', 'integer']:
                            value = int(value)
                        elif param_meta.type in ['float', 'number']:
                            value = float(value)
                        elif param_meta.type in ['bool', 'boolean']:
                            value = value.lower() in ('true', '1', 'yes')

                    params[key] = value

            if params:
                return params
        except Exception:
            pass

        # If only one parameter is required and input is a simple value, use it
        required_params = [p for p in parameters if p.required]
        if len(required_params) == 1:
            param = required_params[0]
            value = input_str.strip('"\'')

            # Convert to appropriate type
            if param.type in ['int', 'integer']:
                try:
                    value = int(value)
                except ValueError:
                    pass
            elif param.type in ['float', 'number']:
                try:
                    value = float(value)
                except ValueError:
                    pass
            elif param.type in ['bool', 'boolean']:
                value = value.lower() in ('true', '1', 'yes')

            return {param.name: value}

        # Return empty dict if parsing failed
        return {}

    def _format_result_for_llm(self, result: Any) -> str:
        """Format the tool result in an LLM-appropriate way."""
        if isinstance(result, dict):
            # Format dict as readable key-value pairs
            formatted_lines = []
            for key, value in result.items():
                if isinstance(value, (dict, list)):
                    formatted_lines.append(
                        f"{key}: {json.dumps(value, indent=2)}")
                else:
                    formatted_lines.append(f"{key}: {value}")
            return "\n".join(formatted_lines)

        elif isinstance(result, list):
            # Format list items
            if not result:
                return "Empty list"
            formatted_items = [f"- {json.dumps(item) if isinstance(item, (dict, list)) else item}"
                               for item in result]
            return "\n".join(formatted_items)

        elif isinstance(result, (str, int, float, bool)):
            # Simple types
            return str(result)

        else:
            # Fallback to JSON
            return json.dumps(result, indent=2)

    def _create_tool_function(self, metadata: ToolMetadata):
        """Create a simple function for the tool that takes a string input."""
        async def tool_func(input_str: str) -> str:
            """Execute the tool by sending a NATS request."""
            try:
                # Parse the input string into parameters
                params = self._parse_tool_input(input_str, metadata.parameters)

                # Validate required parameters
                missing_params = []
                for param in metadata.parameters:
                    if param.required and param.name not in params:
                        missing_params.append(param.name)

                if missing_params:
                    return f"Error: Missing required parameters: {', '.join(missing_params)}"
                # Prepare the request payload
                request_data = json.dumps({
                    "parameters": params
                }).encode()

                # Send request to NATS and wait for response
                response = await self.nc.request(
                    metadata.subject,
                    request_data,
                    timeout=5.0
                )

                # Parse response
                result = json.loads(response.data.decode())

                # Format result for LLM
                return self._format_result_for_llm(result)

            except asyncio.TimeoutError:
                return f"Error: Tool '{metadata.name}' timed out after 5 seconds"
            except Exception as e:
                return f"Error executing tool '{metadata.name}': {str(e)}"

        # Set function name for better debugging
        tool_func.__name__ = metadata.name
        return tool_func

    def _build_tool_description(self, metadata: ToolMetadata) -> str:
        """Build a comprehensive description with parameter details and usage examples."""
        desc_parts = [metadata.description]

        if metadata.parameters:
            desc_parts.append(
                "\n\nInput format: Provide parameters as JSON object or comma-separated key=value pairs")
            desc_parts.append("\nParameters:")

            example_params = {}
            for param in metadata.parameters:
                required_text = "REQUIRED" if param.required else "optional"
                param_desc = f"  - {param.name} ({param.type}, {required_text}): {param.description}"
                if not param.required and param.default is not None:
                    param_desc += f" (default: {param.default})"
                desc_parts.append(param_desc)

                # Build example value
                if param.type in ['int', 'integer']:
                    example_params[param.name] = 42
                elif param.type in ['float', 'number']:
                    example_params[param.name] = 3.14
                elif param.type in ['bool', 'boolean']:
                    example_params[param.name] = True
                else:
                    example_params[param.name] = "example_value"

            # Add example usage
            desc_parts.append(f"\nExample input: {json.dumps(example_params)}")

        return "\n".join(desc_parts)

    def register_tools(self, tools_metadata: List[ToolMetadata]) -> List[Tool]:
        """
        Register discovered tools as LangChain tools.

        Args:
            tools_metadata: List of tool metadata from discovery

        Returns:
            List of Tool instances
        """
        self.tools = []

        for metadata in tools_metadata:
            # Create Pydantic schema for tool parameters
            args_schema = self._create_pydantic_schema(metadata.parameters)

            # Create the tool function
            tool_func = self._create_tool_function(metadata)

            # Build comprehensive description
            description = self._build_tool_description(metadata)

            if 1 == 1:
                # Create simple Tool with string input
                tool = Tool(
                    name=metadata.name,
                    description=description,
                    func=lambda x: "Use async version",  # Placeholder for sync
                    coroutine=tool_func  # Async version
                )

                self.tools.append(tool)
            else:
                # Create structured tool
                tool = StructuredTool(
                    name=metadata.name,
                    description=description,
                    coroutine=tool_func,
                    args_schema=args_schema
                )
                self.tools.append(tool)
            print(f"Registered tool: {metadata.name}")

        return self.tools

    async def initialize(self) -> List[Tool]:
        """
        Initialize the registry by connecting and discovering tools.

        Returns:
            List of registered tools
        """
        await self.connect()
        self.metadata = await self.discover_tools()
        return self.register_tools(self.metadata)
    
    async def getTools(self) -> List[Tool]:
        """
        Initialize the registry by connecting and discovering tools.

        Returns:
            List of registered tools
        """
        await self.connect()
        self.metadata = await self.discover_tools()
        return self.register_tools(self.metadata)


class NATSAgent:
    """LangChain agent that uses dynamic NATS tools."""

    def __init__(
        self,
        llm: BaseChatModel,
        nats_url: str = "nats://localhost:4222",
        verbose: bool = True
    ):
        self.llm = llm
        self.nats_url = nats_url
        self.verbose = verbose
        self.registry = NATSToolRegistry(nats_url)
        self.agent_executor: Optional[AgentExecutor] = None

    async def initialize(self):
        """Initialize the agent by discovering and registering tools."""
        tools = await self.registry.initialize()

        # Create the ReAct agent with clear instructions
        prompt = PromptTemplate.from_template(
            """Answer the following questions as best you can. You have access to the following tools:

{tools}

Use the following format:

Question: the input question you must answer
Thought: you should always think about what to do
Action: the action to take, should be one of [{tool_names}]
Action Input: the input to the action (provide as JSON object with parameter names and values)
Observation: the result of the action
... (this Thought/Action/Action Input/Observation can repeat N times)
Thought: I now know the final answer
Final Answer: the final answer to the original input question

IMPORTANT: For Action Input, provide parameters as a JSON object. For example:
- {{"operation": "multiply", "a": 15, "b": 7}}
- {{"location": "Miami"}}
- {{"text": "Hello World"}}

Begin!

Question: {input}
Thought:{agent_scratchpad}"""
        )

        agent = create_react_agent(self.llm, tools, prompt)
        self.agent_executor = AgentExecutor(
            agent=agent,
            tools=tools,
            verbose=self.verbose,
            handle_parsing_errors=True,
            max_iterations=10
        )

    async def run(self, query: str) -> str:
        """Run the agent with a query."""
        if not self.agent_executor:
            raise RuntimeError(
                "Agent not initialized. Call initialize() first.")

        result = await self.agent_executor.ainvoke({"input": query})
        return result["output"]

    async def cleanup(self):
        """Clean up resources."""
        await self.registry.disconnect()


# Example tool responder implementation
async def example_tool_server():
    """
    Example implementation of NATS tool responders.
    Run this separately to provide tools for the agent.
    """
    nc = await nats.connect("nats://localhost:4222")

    # Tool discovery responder
    async def handle_discovery(msg):
        tools = [
            {
                "name": "calculator",
                "description": "Performs basic arithmetic operations on two numbers",
                "subject": "tools.calculator",
                "parameters": [
                    {
                        "name": "operation",
                        "type": "string",
                        "description": "The operation to perform: add, subtract, multiply, divide",
                        "required": True
                    },
                    {
                        "name": "a",
                        "type": "float",
                        "description": "First number",
                        "required": True
                    },
                    {
                        "name": "b",
                        "type": "float",
                        "description": "Second number",
                        "required": True
                    }
                ],
                "returns": {"type": "object", "description": "Result of the operation"}
            },
            {
                "name": "weather",
                "description": "Gets current weather information for a specified location",
                "subject": "tools.weather",
                "parameters": [
                    {
                        "name": "location",
                        "type": "string",
                        "description": "The city or location name",
                        "required": True
                    }
                ],
                "returns": {"type": "object", "description": "Weather information including temperature, condition, humidity"}
            },
            {
                "name": "string_analyzer",
                "description": "Analyzes a text string and returns statistics about it",
                "subject": "tools.string_analyzer",
                "parameters": [
                    {
                        "name": "text",
                        "type": "string",
                        "description": "The text to analyze",
                        "required": True
                    }
                ],
                "returns": {"type": "object", "description": "Statistics about the text"}
            }
        ]
        await msg.respond(json.dumps(tools).encode())

    # Calculator tool responder
    async def handle_calculator(msg):
        try:
            data = json.loads(msg.data.decode()).get("parameters")
            operation = data["operation"]
            a = float(data["a"])
            b = float(data["b"])

            operations = {
                "add": a + b,
                "subtract": a - b,
                "multiply": a * b,
                "divide": a / b if b != 0 else None
            }

            result = operations.get(operation)

            if result is None and operation == "divide":
                response = {
                    "operation": operation,
                    "a": a,
                    "b": b,
                    "error": "Division by zero"
                }
            elif result is None:
                response = {
                    "operation": operation,
                    "a": a,
                    "b": b,
                    "error": f"Unknown operation: {operation}"
                }
            else:
                response = {
                    "operation": operation,
                    "a": a,
                    "b": b,
                    "result": result
                }

            await msg.respond(json.dumps(response).encode())
        except Exception as e:
            await msg.respond(json.dumps({"error": str(e)}).encode())

    # Weather tool responder (mock)
    async def handle_weather(msg):
        try:
            data = json.loads(msg.data.decode()).get("parameters")
            location = data["location"]

            # Mock weather data
            import random
            conditions = ["Sunny", "Partly Cloudy",
                          "Cloudy", "Rainy", "Stormy"]
            response = {
                "location": location,
                "temperature": random.randint(60, 85),
                "condition": random.choice(conditions),
                "humidity": random.randint(40, 80),
                "wind_speed": random.randint(5, 20),
                "unit": "Fahrenheit"
            }
            await msg.respond(json.dumps(response).encode())
        except Exception as e:
            await msg.respond(json.dumps({"error": str(e)}).encode())

    # String analyzer tool responder
    async def handle_string_analyzer(msg):
        try:
            data = json.loads(msg.data.decode()).get("parameters")
            text = data["text"]

            response = {
                "text_length": len(text),
                "word_count": len(text.split()),
                "character_count": len(text),
                "uppercase_count": sum(1 for c in text if c.isupper()),
                "lowercase_count": sum(1 for c in text if c.islower()),
                "digit_count": sum(1 for c in text if c.isdigit()),
                "whitespace_count": sum(1 for c in text if c.isspace())
            }
            await msg.respond(json.dumps(response).encode())
        except Exception as e:
            await msg.respond(json.dumps({"error": str(e)}).encode())

    # Subscribe to subjects
    await nc.subscribe("tools.discovery", cb=handle_discovery)
    await nc.subscribe("tools.calculator", cb=handle_calculator)
    await nc.subscribe("tools.weather", cb=handle_weather)
    await nc.subscribe("tools.string_analyzer", cb=handle_string_analyzer)

    print("Tool server running...")
    print("- Discovery: tools.discovery")
    print("- Calculator: tools.calculator")
    print("- Weather: tools.weather")
    print("- String Analyzer: tools.string_analyzer")
    print("\nWaiting for requests...")

    # Keep server running
    try:
        await asyncio.Future()
    except KeyboardInterrupt:
        await nc.close()


# Example usage
async def main():
    """Example usage of the NATS agent system."""
    from langchain_openai import ChatOpenAI

    # Create LLM (you'll need to set OPENAI_API_KEY)
    # llm = ChatOpenAI(model="gpt-4", temperature=0)
    url = "https://api.deepseek.com"
    # "sk-3b8842c4b8de41b48ad350662886e849"
    api_key = "sk-fec93ba732c046b38b35263b0a4c004d"
    GPT_OSS = "deepseek-chat"
    llm = ChatOpenAI(model=GPT_OSS, temperature=0,
                     base_url=url, api_key=api_key)
    # Create and initialize agent
    agent = NATSAgent(llm, nats_url="nats://localhost:4222", verbose=True)
    await agent.initialize()

    # Run queries
    print("\n" + "="*50)
    print("Query 1: Calculator")
    print("="*50)
    result = await agent.run("What is 15 multiplied by 7?")
    print(f"\nFinal Result: {result}")

    print("\n" + "="*50)
    print("Query 2: Weather")
    print("="*50)
    result = await agent.run("What's the weather like in Miami?")
    print(f"\nFinal Result: {result}")

    print("\n" + "="*50)
    print("Query 3: String Analysis")
    print("="*50)
    result = await agent.run("Analyze this text: 'Hello World 123!'")
    print(f"\nFinal Result: {result}")

    # Cleanup
    await agent.cleanup()


if __name__ == "__main__":
    # To run the tool server:
    # asyncio.run(example_tool_server())

    # To run the agent:
    asyncio.run(main())
