"""
Hajir Sanat Multi-Agent Sales Assistant
A LangGraph-based chatbot for UPS sales team to calculate prices and generate quotations
"""

from typing import TypedDict, Annotated, List, Dict, Optional, Literal
from langgraph.graph import StateGraph, END
from langgraph.prebuilt import ToolNode
from langchain_core.messages import HumanMessage, AIMessage, SystemMessage, ToolMessage
from langchain_openai import ChatOpenAI
from langchain_core.tools import tool
import operator
import json
from datetime import datetime
import asyncio
import nats
from nats.aio.client import Client as NATS


# ============================================================================
# NATS Integration Layer
# ============================================================================

class NATSClient:
    """Handle NATS communication for product and price queries"""
    
    def __init__(self, nats_url: str = "nats://localhost:4222"):
        self.nats_url = nats_url
        self.nc: Optional[NATS] = None
    
    async def connect(self):
        """Connect to NATS server"""
        if not self.nc:
            self.nc = await nats.connect(self.nats_url)
    
    async def disconnect(self):
        """Disconnect from NATS server"""
        if self.nc:
            await self.nc.close()
    
    async def search_product(self, query: str) -> List[Dict]:
        """Search for products via NATS"""
        await self.connect()
        
        request = json.dumps({'query': query}).encode()
        response = await self.nc.request("searchproduct", request, timeout=5)
        
        return json.loads(response.data.decode())
    
    async def get_product_price(self, product_id: str) -> Dict:
        """Get product price via NATS"""
        await self.connect()
        
        request = json.dumps({'product_id': product_id}).encode()
        response = await self.nc.request("getprice", request, timeout=5)
        
        return json.loads(response.data.decode())


# Global NATS client instance
nats_client = NATSClient()


# ============================================================================
# Tools Definition
# ============================================================================

@tool
async def search_product_tool(query: str) -> str:
    """
    Search for UPS products, batteries, cabinets, or accessories.
    Use this when the user mentions a product type or name.
    
    Args:
        query: Search text (e.g., "10KVA UPS", "battery 12V", "cabinet")
    
    Returns:
        JSON string with search results containing product information
    """
    try:
        results = await nats_client.search_product(query)
        return json.dumps(results, indent=2)
    except Exception as e:
        return json.dumps({"error": f"Search failed: {str(e)}"})


@tool
async def get_price_tool(product_id: str) -> str:
    """
    Get the current price for a specific product by its ID.
    Use this after identifying the exact product from search results.
    
    Args:
        product_id: The unique product identifier
    
    Returns:
        JSON string with price information
    """
    try:
        price_info = await nats_client.get_product_price(product_id)
        return json.dumps(price_info, indent=2)
    except Exception as e:
        return json.dumps({"error": f"Price lookup failed: {str(e)}"})


@tool
def calculate_bundle_total(items: List[Dict[str, any]]) -> str:
    """
    Calculate the total price for a bundle of products.
    
    Args:
        items: List of items, each with 'product_id', 'name', 'price', and 'quantity'
    
    Returns:
        JSON string with subtotal, tax, and total calculations
    """
    try:
        subtotal = sum(item['price'] * item.get('quantity', 1) for item in items)
        tax_rate = 0.09  # 9% tax rate (adjust as needed)
        tax = subtotal * tax_rate
        total = subtotal + tax
        
        return json.dumps({
            "items": items,
            "subtotal": subtotal,
            "tax": tax,
            "tax_rate": tax_rate,
            "total": total,
            "currency": "IRR"  # or "USD" based on company preference
        }, indent=2)
    except Exception as e:
        return json.dumps({"error": f"Calculation failed: {str(e)}"})


@tool
def generate_quotation(
    customer_name: str,
    bundle_calculation: str,
    salesperson_name: str,
    notes: Optional[str] = ""
) -> str:
    """
    Generate a formal quotation document for the customer.
    
    Args:
        customer_name: Name of the customer
        bundle_calculation: JSON string from calculate_bundle_total
        salesperson_name: Name of the salesperson
        notes: Additional notes or terms
    
    Returns:
        Formatted quotation document
    """
    try:
        calc = json.loads(bundle_calculation)
        quote_number = f"HJ-{datetime.now().strftime('%Y%m%d-%H%M%S')}"
        
        quotation = f"""
╔════════════════════════════════════════════════════════════════╗
║                      HAJIR SANAT                               ║
║                  UPS Systems & Solutions                       ║
╚════════════════════════════════════════════════════════════════╝

QUOTATION: {quote_number}
Date: {datetime.now().strftime('%Y-%m-%d %H:%M')}

Customer: {customer_name}
Salesperson: {salesperson_name}

────────────────────────────────────────────────────────────────

ITEMS:
"""
        
        for idx, item in enumerate(calc['items'], 1):
            quantity = item.get('quantity', 1)
            price = item['price']
            line_total = price * quantity
            
            quotation += f"\n{idx}. {item['name']}\n"
            quotation += f"   Product ID: {item['product_id']}\n"
            quotation += f"   Quantity: {quantity} × {price:,.2f} = {line_total:,.2f}\n"
        
        quotation += f"""
────────────────────────────────────────────────────────────────

Subtotal:        {calc['subtotal']:>20,.2f} {calc['currency']}
Tax ({calc['tax_rate']*100:.0f}%):        {calc['tax']:>20,.2f} {calc['currency']}
                 {'─' * 30}
TOTAL:           {calc['total']:>20,.2f} {calc['currency']}

────────────────────────────────────────────────────────────────

Notes: {notes if notes else 'Standard warranty and terms apply'}

Valid for: 30 days from quotation date

────────────────────────────────────────────────────────────────

Thank you for choosing Hajir Sanat!
For questions, please contact your salesperson.

"""
        return quotation
    except Exception as e:
        return f"Error generating quotation: {str(e)}"


# ============================================================================
# State Definition
# ============================================================================

class AgentState(TypedDict):
    """State for the sales assistant workflow"""
    messages: Annotated[List, operator.add]
    bundle_items: List[Dict]  # Collected products with prices
    current_step: str  # workflow tracking
    pending_clarification: Optional[Dict]  # When multiple products found
    quotation_requested: bool
    customer_info: Optional[Dict]


# ============================================================================
# Agent Nodes
# ============================================================================

def create_llm():
    """Create LLM instance with tools"""
    return ChatOpenAI(model="gpt-4o", temperature=0)


async def conversation_router(state: AgentState) -> str:
    """Route conversation based on current context"""
    messages = state["messages"]
    last_message = messages[-1].content.lower() if messages else ""
    
    # Check for quotation request
    if any(keyword in last_message for keyword in ["quotation", "quote", "proposal", "document"]):
        if state["bundle_items"]:
            return "generate_quotation"
        else:
            return "need_products_first"
    
    # Check if we need clarification
    if state.get("pending_clarification"):
        return "handle_clarification"
    
    # Check if we have items and need to calculate
    if state["bundle_items"] and any(keyword in last_message for keyword in ["total", "calculate", "price", "cost"]):
        return "calculate_price"
    
    # Default to product gathering
    return "gather_products"


async def gather_products_agent(state: AgentState) -> AgentState:
    """Agent focused on identifying and gathering products for the bundle"""
    
    system_prompt = """You are a helpful sales assistant for Hajir Sanat, a UPS manufacturing company.

Your current task is to help the salesperson build a product bundle for their customer.

A typical UPS bundle includes:
1. UPS unit (main product)
2. Batteries (usually multiple units)
3. Cabinet (optional, for housing)
4. Accessories (cables, connectors, monitoring cards, etc.)

Guidelines:
- Ask clarifying questions about the customer's power requirements
- Use search_product_tool to find relevant products
- If multiple products match, present them clearly and ask for selection
- Keep track of quantities for each item
- Be conversational and helpful

When you have identified products, confirm with the user before moving forward.
"""
    
    llm = create_llm().bind_tools([search_product_tool, get_price_tool])
    
    messages = [SystemMessage(content=system_prompt)] + state["messages"]
    response = await llm.ainvoke(messages)
    
    # Handle tool calls
    if response.tool_calls:
        state["messages"].append(response)
        return state
    
    # Regular message
    state["messages"].append(response)
    state["current_step"] = "gathering_products"
    
    return state


async def handle_clarification_agent(state: AgentState) -> AgentState:
    """Handle cases where multiple products match and user needs to choose"""
    
    clarification_data = state["pending_clarification"]
    
    system_prompt = """You are helping the user select from multiple product options.

Present the options clearly with:
- Product name
- Product ID
- Key specifications
- Price (if available)

Ask the user to specify which one they want by number or ID.
"""
    
    llm = create_llm()
    
    messages = [SystemMessage(content=system_prompt)] + state["messages"]
    response = await llm.ainvoke(messages)
    
    state["messages"].append(response)
    
    # Clear pending clarification after handling
    if "select" in state["messages"][-2].content.lower() or any(char.isdigit() for char in state["messages"][-2].content):
        state["pending_clarification"] = None
    
    return state


async def calculate_price_agent(state: AgentState) -> AgentState:
    """Calculate total price for the bundle"""
    
    system_prompt = """You are calculating the total price for a UPS bundle.

Use the calculate_bundle_total tool with the bundle items collected so far.
Present the results clearly to the salesperson, breaking down:
- Individual items and quantities
- Subtotal
- Tax
- Total amount

Ask if they want to make any changes or proceed to generate a quotation.
"""
    
    llm = create_llm().bind_tools([calculate_bundle_total])
    
    messages = [SystemMessage(content=system_prompt)] + state["messages"]
    
    # Add bundle context
    bundle_context = f"\n\nCurrent bundle items: {json.dumps(state['bundle_items'], indent=2)}"
    messages.append(HumanMessage(content=bundle_context))
    
    response = await llm.ainvoke(messages)
    state["messages"].append(response)
    state["current_step"] = "price_calculated"
    
    return state


async def quotation_agent(state: AgentState) -> AgentState:
    """Generate formal quotation document"""
    
    system_prompt = """You are generating a formal quotation for the customer.

First, collect necessary information:
- Customer name (if not already provided)
- Salesperson name
- Any special notes or terms

Then use the generate_quotation tool to create the document.
"""
    
    llm = create_llm().bind_tools([generate_quotation])
    
    messages = [SystemMessage(content=system_prompt)] + state["messages"]
    
    # Add bundle and calculation context
    context = f"\n\nBundle items with prices: {json.dumps(state['bundle_items'], indent=2)}"
    messages.append(HumanMessage(content=context))
    
    response = await llm.ainvoke(messages)
    state["messages"].append(response)
    state["current_step"] = "quotation_generated"
    state["quotation_requested"] = False
    
    return state


def need_products_message(state: AgentState) -> AgentState:
    """Message when quotation requested but no products yet"""
    message = AIMessage(content="I'd be happy to generate a quotation, but we need to select products first. What kind of UPS system is your customer looking for? Please tell me about their power requirements.")
    state["messages"].append(message)
    state["quotation_requested"] = False
    return state


# ============================================================================
# Tool Execution Node
# ============================================================================

async def tool_executor(state: AgentState) -> AgentState:
    """Execute tools called by agents"""
    
    last_message = state["messages"][-1]
    
    if not hasattr(last_message, 'tool_calls') or not last_message.tool_calls:
        return state
    
    for tool_call in last_message.tool_calls:
        tool_name = tool_call["name"]
        tool_args = tool_call["args"]
        
        # Execute the appropriate tool
        if tool_name == "search_product_tool":
            result = await search_product_tool.ainvoke(tool_args)
            
            # Check if multiple results returned
            try:
                results = json.loads(result)
                if isinstance(results, list) and len(results) > 1:
                    state["pending_clarification"] = {
                        "type": "product_selection",
                        "options": results
                    }
            except:
                pass
                
        elif tool_name == "get_price_tool":
            result = await get_price_tool.ainvoke(tool_args)
            
            # Add to bundle items if successful
            try:
                price_data = json.loads(result)
                if "error" not in price_data:
                    # Add to bundle
                    state["bundle_items"].append({
                        "product_id": tool_args["product_id"],
                        "name": price_data.get("name", "Unknown"),
                        "price": price_data.get("price", 0),
                        "quantity": 1
                    })
            except:
                pass
                
        elif tool_name == "calculate_bundle_total":
            result = calculate_bundle_total.invoke(tool_args)
            
        elif tool_name == "generate_quotation":
            result = generate_quotation.invoke(tool_args)
        else:
            result = f"Unknown tool: {tool_name}"
        
        # Add tool result to messages
        tool_message = ToolMessage(
            content=result,
            tool_call_id=tool_call["id"]
        )
        state["messages"].append(tool_message)
    
    return state


def should_continue(state: AgentState) -> str:
    """Determine if we should continue processing or end"""
    
    last_message = state["messages"][-1]
    
    # If there are tool calls, execute them
    if hasattr(last_message, 'tool_calls') and last_message.tool_calls:
        return "execute_tools"
    
    # Check if conversation should end
    if state["current_step"] == "quotation_generated":
        return "end"
    
    # Continue conversation
    return "route"


# ============================================================================
# Graph Construction
# ============================================================================

def create_sales_assistant_graph():
    """Create the LangGraph workflow for the sales assistant"""
    
    workflow = StateGraph(AgentState)
    
    # Add nodes
    workflow.add_node("gather_products", gather_products_agent)
    workflow.add_node("handle_clarification", handle_clarification_agent)
    workflow.add_node("calculate_price", calculate_price_agent)
    workflow.add_node("generate_quotation", quotation_agent)
    workflow.add_node("need_products_first", need_products_message)
    workflow.add_node("execute_tools", tool_executor)
    
    # Set entry point
    workflow.set_entry_point("gather_products")
    
    # Add conditional edges
    workflow.add_conditional_edges(
        "gather_products",
        should_continue,
        {
            "execute_tools": "execute_tools",
            "route": "gather_products",
            "end": END
        }
    )
    
    workflow.add_conditional_edges(
        "execute_tools",
        should_continue,
        {
            "execute_tools": "execute_tools",
            "route": "gather_products",
            "end": END
        }
    )
    
    workflow.add_edge("handle_clarification", "gather_products")
    workflow.add_edge("calculate_price", "gather_products")
    workflow.add_edge("generate_quotation", END)
    workflow.add_edge("need_products_first", "gather_products")
    
    return workflow.compile()


# ============================================================================
# Main Application
# ============================================================================

async def main():
    """Main application entry point"""
    
    # Initialize NATS connection
    await nats_client.connect()
    
    # Create the graph
    app = create_sales_assistant_graph()
    
    # Initialize state
    initial_state = {
        "messages": [
            HumanMessage(content="Hello! I need help preparing a quote for a customer.")
        ],
        "bundle_items": [],
        "current_step": "initial",
        "pending_clarification": None,
        "quotation_requested": False,
        "customer_info": None
    }
    
    print("Hajir Sanat Sales Assistant")
    print("=" * 60)
    print("Type 'quit' to exit\n")
    
    # Run conversation loop
    state = initial_state
    
    while True:
        # Process current state
        async for event in app.astream(state):
            for node_name, node_state in event.items():
                if "messages" in node_state and node_state["messages"]:
                    last_msg = node_state["messages"][-1]
                    if isinstance(last_msg, AIMessage):
                        print(f"\nAssistant: {last_msg.content}")
        
        # Get user input
        user_input = input("\nYou: ").strip()
        
        if user_input.lower() in ['quit', 'exit', 'bye']:
            print("\nThank you for using Hajir Sanat Sales Assistant!")
            break
        
        # Add user message to state
        state["messages"].append(HumanMessage(content=user_input))
    
    # Cleanup
    await nats_client.disconnect()


if __name__ == "__main__":
    asyncio.run(main())