"""
    Version 1.0
"""
import json
import asyncio
from typing import Any, Dict, List, Optional, Type, Callable
# from langchain.agents import AgentExecutor, create_tool_calling_agent
from langchain_classic.agents import AgentExecutor, create_tool_calling_agent
from langchain_openai import ChatOpenAI
from langchain_core.messages import HumanMessage, AIMessage
from langchain_core.prompts import ChatPromptTemplate, MessagesPlaceholder
from dataclasses import dataclass
from pydantic import BaseModel, Field, create_model
from langchain_classic.tools import Tool, StructuredTool
from lib import bus, MsgContext, Subjects, BaseAgent, cache, CacheStrategy
import models
from models import ToolMetadata, ToolParameter

import logging
logger = logging.getLogger(__name__)


class ToolRegistry:
    """Registry for managing dynamic NATS-based tools."""

    def __init__(self):
        # self.nats_url = nats_url
        # self.nc: Optional[NATS] = None
        self.tools: List[Tool] = []
        self.discovery_subject = Subjects.AI.Agents.Mamagements.listools  # "tools.discovery"
        self.metadata: List[ToolMetadata] = None

    async def connect(self):
        """Connect to NATS server."""
        # self.nc = await nats.connect(self.nats_url)
        # print(f"Connected to NATS at {self.nats_url}")

    async def disconnect(self):
        """Disconnect from NATS server."""
        # if self.nc:
        #     await self.nc.close()
        #     print("Disconnected from NATS")

    async def discover_tools(self, timeout: float = 5.0) -> List[ToolMetadata]:
        """
        Discover available tools by sending a discovery request.

        Returns:
            List of ToolMetadata objects describing available tools
        """
        # if not self.nc:
        #     raise RuntimeError("Not connected to NATS. Call connect() first.")

        try:
            logger.info("Discovering Tools")
            # Send discovery request
            # response = await self.nc.request(
            #     self.discovery_subject,
            #     b"",
            #     timeout=timeout
            # )
            response: MsgContext = await bus.request(self.discovery_subject, {},)

            # Parse response
            tools_data = json.loads(response.msg.data.decode())

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

    def _create_tool_function(self, metadata: ToolMetadata, context: models.SessionContext = None):
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
                # # Prepare the request payload
                # request_data = json.dumps({
                #     "parameters": params,
                #     "context":context
                # }).encode()

                # Send request to NATS and wait for response

                data = models.ToolContext(params=params, context=context)
                response: MsgContext = await bus.request(
                    metadata.subject,
                    data,
                    timeout=15.0
                )

                # response = await self.nc.request(
                #     metadata.subject,
                #     request_data,
                #     timeout=5.0
                # )

                # Parse response
                result = json.loads(response.msg.data.decode())

                # Format result for LLM
                return self._format_result_for_llm(result)

            except asyncio.TimeoutError:
                return f"Error: Tool '{metadata.name}' timed out after 5 seconds"
            except Exception as e:
                return f"Error executing tool '{metadata.name}': {str(e)}"

        async def tool_func_ex(*args, **kwargs):
            params = {}

            if args:
                for i, arg in enumerate(args):
                    params[f"arg_{i}"] = arg
            if kwargs:
                params.update(kwargs)
            missing_params = []
            for param in metadata.parameters:
                if param.required and param.name not in params:
                    missing_params.append(param.name)
            if missing_params:
                return f"Error: Missing required parameters: {', '.join(missing_params)}"
            data = models.ToolContext(params=params, context=context)
            response: MsgContext = await bus.request(
                metadata.subject,
                data,
                timeout=15.0
            )
            result = json.loads(response.msg.data.decode())
            return self._format_result_for_llm(result)

        # Set function name for better debugging
        tool_func.__name__ = metadata.name
        tool_func_ex.__name__ = metadata.name
        return tool_func_ex

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

    def register_tools(self, tools_metadata: List[ToolMetadata], context: models.SessionContext = None) -> List[Tool]:
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
            tool_func = self._create_tool_function(metadata, context)

            # Build comprehensive description
            description = self._build_tool_description(metadata)

            if 1 == 0:
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
        # await self.connect()
        print("here")
        self.metadata = await self.discover_tools()
        return self.register_tools(self.metadata)

    async def getTools(self, refersh: bool = False,
                       callable: Optional[Callable[[
                           ToolMetadata], bool]] = None,
                       context: models.SessionContext = None) -> List[Tool]:
        """
        Initialize the registry by connecting and discovering tools.

        Returns:
            List of registered tools
        """
        # await self.connect()
        if self.metadata == None or refersh:
            self.metadata = await self.discover_tools()
        if self.tools == None or refersh:
            self.register_tools(self.metadata, context)
        return self.tools

    def getToolByName(self, name: str) -> Tool:
        return next((x for x in self.tools if x.name == name), None)


class LangchainAgent(BaseAgent):
    def __init__(self, name: str, description: str, system_prompt: str):
        super().__init__(name, description)
        self._executor: AgentExecutor = None
        self._llm: ChatOpenAI = None
        self.system_prompt: str = system_prompt

    def ToMessage(self, msg: models.ChatMessage):
        return HumanMessage(msg.content[0].get('text')) if msg.role == 'user' else AIMessage(msg.content[0].get('text'))

    async def llm(self) -> ChatOpenAI:
        if (self._llm == None):
            params: models.LLMParameters = (await self.get_llm_params())
            logger.info(
                f"LLM successfully initialize. Name:{params.Name}, Model:{params.Model}, Url:{params.Url}")
            self._llm = ChatOpenAI(model=params.Model, temperature=0,
                                   base_url=params.Url, api_key=params.ApiKey)
        return self._llm

    async def getTools(self, ctx: models.SessionContext = None) -> List[Tool]:
        return []

    async def create_executor(self, context: models.SessionContext = None) -> AgentExecutor:
        llm = await self.llm()
        prompt = ChatPromptTemplate.from_messages([
            ("system", self.system_prompt),
            MessagesPlaceholder(variable_name="chat_history"),
            ("human", "{input}"),
            ("placeholder", "{agent_scratchpad}"),
        ])
        tools = await self.getTools(context)
        agent = create_tool_calling_agent(llm, tools, prompt)
        return AgentExecutor(
            agent=agent,
            tools=tools,
            verbose=True,
            handle_parsing_errors=True
        )

    async def executor(self, ctx: models.SessionContext = None, refresh: bool = False):
        # if ctx is not None:
        #     ret = cache.get(ctx.SessionId, value_provider=None,
        #                     strategy=CacheStrategy.TTL, ttl_seconds=60)
        #     if ret is None:
        #         v = await self.create_executor(ctx)
        #         ret = cache.get(
        #             ctx.SessionId, value_provider=lambda: v,  strategy=CacheStrategy.TTL, ttl_seconds=60)
        #     return ret
        if self._executor is None:
            self._executor = await self.create_executor(context=ctx)
        return self._executor

    async def reply(self, req: models.AgentRequest):
        logger.info(f"{self.name} starts.")
        _history = [self.ToMessage(msg)
                    for msg in req.chat_history]
        ex = await self.executor(req.context)
        response = await ex.ainvoke({
            "input": req.input_text,
            "chat_history": _history
        })

        return models.AgentResponse(text=response['output'])


async def get_llm() -> ChatOpenAI:
    res: MsgContext = await bus.request(Subjects.AI.Agents.Mamagements.get_available_llms, {})
    params = res.GetPayload(models.GetAvailableLLMsResponse)
    ChatOpenAI(model=params.Model, temperature=0,
               base_url=params.Url, api_key=params.ApiKey)


toolsRegistry = ToolRegistry()
