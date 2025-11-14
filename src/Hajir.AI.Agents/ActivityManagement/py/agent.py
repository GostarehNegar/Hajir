"""
Hajir Sanat Activity Management Assistant using LangGraph
A multiagent system for registering and tracking user activities
"""

from typing import TypedDict, Annotated, Sequence, List, Dict, Any, Optional
from datetime import datetime
import operator
from langgraph.graph import StateGraph, END
from langchain_core.messages import BaseMessage, HumanMessage, AIMessage, ToolMessage
from langchain_core.tools import tool,BaseTool
from lib import bus, BaseAgent
from langchain_openai import ChatOpenAI
from lib_langchain import toolsRegistry
import json
import models
import logging
from dataclasses import dataclass
logger = logging.getLogger(__name__)

# ============================================================================
# STATE DEFINITION
# ============================================================================

class AgentState(TypedDict):
    """State shared across all agents in the system"""
    messages: Annotated[Sequence[BaseMessage], operator.add]
    current_user_id: str
    recent_contacts: List[Dict[str, Any]]
    recent_activities: List[Dict[str, Any]]
    next_agent: Optional[str]

# ============================================================================
# ACTIVITY REGISTRATION TOOLS
# ============================================================================

llm:ChatOpenAI = None

async def createApp(ctx:models.SessionContext):

    @tool
    def register_phone_call(
        contact_id: str,
        direction: str,
        duration_minutes: int,
        notes: str,
        outcome: str
    ) -> str:
        """
        Register a phone call activity.
        
        Args:
            contact_id: ID of the contact involved in the call
            direction: Either 'incoming' or 'outgoing'
            duration_minutes: Duration of the call in minutes
            notes: Notes about the call content
            outcome: Outcome of the call (e.g., 'successful', 'no_answer', 'follow_up_needed')
        
        Returns:
            Confirmation message with activity ID
        """
        # Sample implementation - will be replaced with actual database call
        activity_id = f"CALL_{datetime.now().strftime('%Y%m%d%H%M%S')}"
        logger.info(f"============ Owner {ctx.UserId}")
        activity_data = {
            "activity_id": activity_id,
            "type": "phone_call",
            "contact_id": contact_id,
            "direction": direction,
            "duration_minutes": duration_minutes,
            "notes": notes,
            "outcome": outcome,
            "timestamp": datetime.now().isoformat(),
            "status": "completed"
        }
        
        # TODO: Save to database
        print(f"[DB] Saving phone call: {json.dumps(activity_data, indent=2)}")
        
        return f"Phone call registered successfully. Activity ID: {activity_id}"


    @tool
    def register_email(
        contact_id: str,
        direction: str,
        subject: str,
        summary: str,
        has_attachment: bool = False
    ) -> str:
        """
        Register an email activity.
        
        Args:
            contact_id: ID of the contact involved
            direction: Either 'received' or 'sent'
            subject: Email subject line
            summary: Brief summary of email content
            has_attachment: Whether email has attachments
        
        Returns:
            Confirmation message with activity ID
        """
        activity_id = f"EMAIL_{datetime.now().strftime('%Y%m%d%H%M%S')}"
        
        activity_data = {
            "activity_id": activity_id,
            "type": "email",
            "contact_id": contact_id,
            "direction": direction,
            "subject": subject,
            "summary": summary,
            "has_attachment": has_attachment,
            "timestamp": datetime.now().isoformat(),
            "status": "completed"
        }
        
        # TODO: Save to database
        print(f"[DB] Saving email: {json.dumps(activity_data, indent=2)}")
        
        return f"Email registered successfully. Activity ID: {activity_id}"


    @tool
    def register_sms(
        contact_id: str,
        direction: str,
        message_content: str
    ) -> str:
        """
        Register an SMS activity.
        
        Args:
            contact_id: ID of the contact involved
            direction: Either 'received' or 'sent'
            message_content: Content of the SMS message
        
        Returns:
            Confirmation message with activity ID
        """
        activity_id = f"SMS_{datetime.now().strftime('%Y%m%d%H%M%S')}"
        
        activity_data = {
            "activity_id": activity_id,
            "type": "sms",
            "contact_id": contact_id,
            "direction": direction,
            "message_content": message_content,
            "timestamp": datetime.now().isoformat(),
            "status": "completed"
        }
        
        # TODO: Save to database
        print(f"[DB] Saving SMS: {json.dumps(activity_data, indent=2)}")
        
        return f"SMS registered successfully. Activity ID: {activity_id}"


    @tool
    def register_meeting(
        contact_ids: List[str],
        meeting_type: str,
        location: str,
        duration_minutes: int,
        agenda: str,
        notes: str,
        scheduled_datetime: Optional[str] = None
    ) -> str:
        """
        Register a meeting activity.
        
        Args:
            contact_ids: List of contact IDs who attended
            meeting_type: Type of meeting (e.g., 'in_person', 'video_call', 'phone_conference')
            location: Meeting location or platform
            duration_minutes: Duration in minutes
            agenda: Meeting agenda
            notes: Meeting notes and outcomes
            scheduled_datetime: ISO format datetime if scheduling future meeting
        
        Returns:
            Confirmation message with activity ID
        """
        activity_id = f"MEET_{datetime.now().strftime('%Y%m%d%H%M%S')}"
        
        activity_data = {
            "activity_id": activity_id,
            "type": "meeting",
            "contact_ids": contact_ids,
            "meeting_type": meeting_type,
            "location": location,
            "duration_minutes": duration_minutes,
            "agenda": agenda,
            "notes": notes,
            "scheduled_datetime": scheduled_datetime or datetime.now().isoformat(),
            "timestamp": datetime.now().isoformat(),
            "status": "completed" if not scheduled_datetime else "scheduled"
        }
        
        # TODO: Save to database
        print(f"[DB] Saving meeting: {json.dumps(activity_data, indent=2)}")
        
        return f"Meeting registered successfully. Activity ID: {activity_id}"


    # ============================================================================
    # CONTACT MANAGEMENT TOOLS
    # ============================================================================

    @tool
    def search_contacts(
        query: str,
        search_type: str = "all"
    ) -> str:
        """
        Search for contacts by name, company, email, or phone.
        
        Args:
            query: Search query string
            search_type: Type of search ('name', 'company', 'email', 'phone', 'all')
        
        Returns:
            JSON string with list of matching contacts
        """
        # Sample implementation - will be replaced with actual database query
        sample_contacts = [
            {
                "contact_id": "CONT_001",
                "name": "علی احمدی",
                "company": "شرکت صنعتی پارس",
                "email": "ali.ahmadi@example.com",
                "phone": "+98-912-345-6789",
                "position": "مدیر خرید"
            },
            {
                "contact_id": "CONT_002",
                "name": "سارا محمدی",
                "company": "گروه تولیدی آریا",
                "email": "sara.mohammadi@example.com",
                "phone": "+98-913-456-7890",
                "position": "مدیر فروش"
            }
        ]
        
        # Filter based on query (simple implementation)
        results = [c for c in sample_contacts if query.lower() in c["name"].lower() 
                or query.lower() in c["company"].lower()]
        
        print(f"[DB] Searching contacts with query: '{query}', type: '{search_type}'")
        
        return json.dumps({
            "query": query,
            "search_type": search_type,
            "results": results,
            "count": len(results)
        }, ensure_ascii=False, indent=2)


    @tool
    def get_contact_details(contact_id: str) -> str:
        """
        Get detailed information about a specific contact.
        
        Args:
            contact_id: The contact's unique identifier
        
        Returns:
            JSON string with contact details
        """
        # Sample implementation
        sample_contact = {
            "contact_id": contact_id,
            "name": "علی احمدی",
            "company": "شرکت صنعتی پارس",
            "email": "ali.ahmadi@example.com",
            "phone": "+98-912-345-6789",
            "position": "مدیر خرید",
            "address": "تهران، خیابان ولیعصر",
            "created_date": "2024-01-15",
            "last_activity": "2024-11-10",
            "tags": ["مشتری مهم", "صنعت فلزات"]
        }
        
        print(f"[DB] Fetching contact details for: {contact_id}")
        
        return json.dumps(sample_contact, ensure_ascii=False, indent=2)


    @tool
    def get_recent_contacts(user_id: str, limit: int = 10) -> str:
        """
        Get list of contacts recently interacted with by the user.
        
        Args:
            user_id: The user's unique identifier
            limit: Maximum number of contacts to return (default: 10)
        
        Returns:
            JSON string with list of recent contacts
        """
        # Sample implementation
        recent_contacts = [
            {
                "contact_id": "CONT_001",
                "name": "علی احمدی",
                "company": "شرکت صنعتی پارس",
                "last_interaction": "2024-11-12",
                "interaction_type": "phone_call"
            },
            {
                "contact_id": "CONT_003",
                "name": "رضا کریمی",
                "company": "تجارت نوین",
                "last_interaction": "2024-11-11",
                "interaction_type": "meeting"
            },
            {
                "contact_id": "CONT_002",
                "name": "سارا محمدی",
                "company": "گروه تولیدی آریا",
                "last_interaction": "2024-11-10",
                "interaction_type": "email"
            }
        ]
        
        print(f"[DB] Fetching recent contacts for user: {user_id}, limit: {limit}")
        
        return json.dumps({
            "user_id": user_id,
            "contacts": recent_contacts[:limit],
            "count": len(recent_contacts[:limit])
        }, ensure_ascii=False, indent=2)


    # ============================================================================
    # ACTIVITY TRACKING TOOLS
    # ============================================================================

    @tool
    def get_user_tasks(
        user_id: str,
        status: str = "all",
        limit: int = 20
    ) -> str:
        """
        Get current tasks for the user.
        
        Args:
            user_id: The user's unique identifier
            status: Filter by status ('pending', 'in_progress', 'completed', 'all')
            limit: Maximum number of tasks to return
        
        Returns:
            JSON string with list of tasks
        """
        # Sample implementation
        sample_tasks = [
            {
                "task_id": "TASK_001",
                "title": "پیگیری پیشنهاد قیمت برای شرکت پارس",
                "description": "ارسال پیشنهاد قیمت محصولات جدید",
                "status": "pending",
                "priority": "high",
                "due_date": "2024-11-15",
                "related_contact": "CONT_001",
                "created_date": "2024-11-12"
            },
            {
                "task_id": "TASK_002",
                "title": "جلسه هماهنگی با تیم فروش",
                "description": "بررسی عملکرد فروش ماه اخیر",
                "status": "in_progress",
                "priority": "medium",
                "due_date": "2024-11-14",
                "created_date": "2024-11-10"
            }
        ]
        
        # Filter by status
        if status != "all":
            filtered_tasks = [t for t in sample_tasks if t["status"] == status]
        else:
            filtered_tasks = sample_tasks
        
        print(f"[DB] Fetching tasks for user: {user_id}, status: {status}")
        
        return json.dumps({
            "user_id": user_id,
            "status_filter": status,
            "tasks": filtered_tasks[:limit],
            "count": len(filtered_tasks[:limit])
        }, ensure_ascii=False, indent=2)


    @tool
    def get_recent_activities(
        user_id: str,
        activity_type: str = "all",
        days: int = 7
    ) -> str:
        """
        Get recent activities for the user.
        
        Args:
            user_id: The user's unique identifier
            activity_type: Filter by type ('phone_call', 'email', 'sms', 'meeting', 'all')
            days: Number of days to look back (default: 7)
        
        Returns:
            JSON string with list of activities
        """
        # Sample implementation
        sample_activities = [
            {
                "activity_id": "CALL_20241112143022",
                "type": "phone_call",
                "contact_name": "علی احمدی",
                "direction": "incoming",
                "timestamp": "2024-11-12T14:30:22",
                "duration_minutes": 15,
                "notes": "بحث در مورد محصولات جدید"
            },
            {
                "activity_id": "EMAIL_20241111095033",
                "type": "email",
                "contact_name": "سارا محمدی",
                "direction": "sent",
                "timestamp": "2024-11-11T09:50:33",
                "subject": "پیشنهاد همکاری"
            },
            {
                "activity_id": "MEET_20241110100044",
                "type": "meeting",
                "contact_name": "رضا کریمی",
                "timestamp": "2024-11-10T10:00:44",
                "duration_minutes": 60,
                "location": "دفتر مرکزی"
            }
        ]
        
        # Filter by activity type
        if activity_type != "all":
            filtered_activities = [a for a in sample_activities if a["type"] == activity_type]
        else:
            filtered_activities = sample_activities
        
        print(f"[DB] Fetching activities for user: {user_id}, type: {activity_type}, days: {days}")
        
        return json.dumps({
            "user_id": user_id,
            "activity_type": activity_type,
            "days": days,
            "activities": filtered_activities,
            "count": len(filtered_activities)
        }, ensure_ascii=False, indent=2)


    # ============================================================================
    # AGENT DEFINITIONS
    # ============================================================================

    # Collect all tools
    all_tools = [
        register_phone_call,
        register_email,
        register_sms,
        register_meeting,
        get_contact_details,
        get_recent_contacts,
        get_user_tasks,
        get_recent_activities,
        toolsRegistry.getToolByName("searchcontacts")
    ]


    def create_activity_agent(llm):
        """Agent responsible for registering activities"""
        return llm.bind_tools([
            register_phone_call,
            register_email,
            register_sms,
            register_meeting,
            toolsRegistry.getToolByName("searchcontacts")
        ])


    def create_contact_agent(llm):
        """Agent responsible for finding and managing contacts"""
        return llm.bind_tools([
            toolsRegistry.getToolByName("searchcontacts"),
            get_contact_details,
            get_recent_contacts
        ])


    def create_tracking_agent(llm):
        """Agent responsible for tracking and retrieving user activities"""
        return llm.bind_tools([
            get_user_tasks,
            get_recent_activities
        ])


    # ============================================================================
    # ROUTER AND AGENT NODES
    # ============================================================================

    def route_request(state: AgentState) -> str:
        """Route the request to the appropriate agent"""
        messages = state["messages"]
        last_message = messages[-1].content.lower()
        
        # Simple routing logic based on keywords
        if any(keyword in last_message for keyword in ["register", "log", "record", "ثبت", "تماس", "ایمیل", "جلسه"]):
            return "activity_agent"
        elif any(keyword in last_message for keyword in ["find", "search", "contact", "جستجو", "مخاطب", "شخص"]):
            return "contact_agent"
        elif any(keyword in last_message for keyword in ["task", "activity", "recent", "وظیفه", "فعالیت", "اخیر"]):
            return "tracking_agent"
        else:
            return "activity_agent"  # Default to activity agent


    def activity_agent_node(state: AgentState):
        """Node for activity registration agent"""
        #llm = await get_llm() # ChatAnthropic(model="claude-sonnet-4-20250514", temperature=0)
        agent = create_activity_agent(llm)
        
        system_message = """You are an activity registration assistant for Hajir Sanat.
        Your role is to help users register their business activities including phone calls, emails, SMS, and meetings.
        
        When registering activities:
        1. Ask for missing required information politely
        2. If a contact is mentioned but not identified, suggest using the contact search tool
        3. Confirm successful registration with details
        4. Be concise and professional
        
        Always respond in Persian (Farsi) when the user speaks in Persian."""
        
        messages = [HumanMessage(content=system_message)] + state["messages"]
        response = agent.invoke(messages)
        
        return {"messages": [response]}


    def contact_agent_node(state: AgentState):
        """Node for contact management agent"""
        #llm = ChatAnthropic(model="claude-sonnet-4-20250514", temperature=0)
        agent = create_contact_agent(llm)
        
        system_message = f"""You are a contact management assistant for Hajir Sanat.
        Your role is to help users find and retrieve contact information.
        
        Current user ID: {state['current_user_id']}
        
        When helping with contacts:
        1. Use searchcontacts for finding contacts by name or company
        2. Use get_recent_contacts to show recently interacted contacts
        3. Use get_contact_details for full contact information
        4. Be helpful in narrowing down search results
        
        Always respond in Persian (Farsi) when the user speaks in Persian."""
        
        messages = [HumanMessage(content=system_message)] + state["messages"]
        response = agent.invoke(messages)
        
        return {"messages": [response]}


    def tracking_agent_node(state: AgentState):
        """Node for activity tracking agent"""
        #llm = ChatAnthropic(model="claude-sonnet-4-20250514", temperature=0)
        agent = create_tracking_agent(llm)
        
        system_message = f"""You are an activity tracking assistant for Hajir Sanat.
        Your role is to help users view their tasks and recent activities.
        
        Current user ID: {state['current_user_id']}
        
        When providing information:
        1. Use get_user_tasks to show pending, in-progress, or completed tasks
        2. Use get_recent_activities to show recent business activities
        3. Present information clearly and organized
        4. Highlight important or urgent items
        
        Always respond in Persian (Farsi) when the user speaks in Persian."""
        
        messages = [HumanMessage(content=system_message)] + state["messages"]
        response = agent.invoke(messages)
        
        return {"messages": [response]}


    def tool_execution_node(state: AgentState):
        """Execute any tool calls from the agent"""
        messages = state["messages"]
        last_message = messages[-1]
        
        tool_calls = getattr(last_message, "tool_calls", [])
        
        if not tool_calls:
            return {"messages": []}
        
        tool_results = []
        
        # Create a mapping of tool names to functions
        tool_map = {tool.name: tool for tool in all_tools}
        
        for tool_call in tool_calls:
            tool_name = tool_call["name"]
            tool_args = tool_call["args"]
            
            if tool_name in tool_map:
                try:
                    result = tool_map[tool_name].ainvoke(tool_args)
                    tool_results.append(
                        ToolMessage(
                            content=result,
                            tool_call_id=tool_call["id"]
                        )
                    )
                except Exception as e:
                    tool_results.append(
                        ToolMessage(
                            content=f"Error executing {tool_name}: {str(e)}",
                            tool_call_id=tool_call["id"]
                        )
                    )
        
        return {"messages": tool_results}


    def should_continue(state: AgentState) -> str:
        """Determine if we should continue to tool execution or end"""
        messages = state["messages"]
        last_message = messages[-1]
        
        if hasattr(last_message, "tool_calls") and last_message.tool_calls:
            return "tools"
        
        return "end"


    # ============================================================================
    # GRAPH CONSTRUCTION
    # ============================================================================

    def create_workflow():
        """Create the LangGraph workflow"""
        workflow = StateGraph(AgentState)
        
        # Add nodes
        workflow.add_node("activity_agent", activity_agent_node)
        workflow.add_node("contact_agent", contact_agent_node)
        workflow.add_node("tracking_agent", tracking_agent_node)
        workflow.add_node("tools", tool_execution_node)
        
        # Set entry point with routing
        workflow.set_conditional_entry_point(
            route_request,
            {
                "activity_agent": "activity_agent",
                "contact_agent": "contact_agent",
                "tracking_agent": "tracking_agent"
            }
        )
        
        # Add conditional edges from each agent
        for agent_name in ["activity_agent", "contact_agent", "tracking_agent"]:
            workflow.add_conditional_edges(
                agent_name,
                should_continue,
                {
                    "tools": "tools",
                    "end": END
                }
            )
        
        # After tool execution, route back to appropriate agent
        workflow.add_conditional_edges(
            "tools",
            route_request,
            {
                "activity_agent": "activity_agent",
                "contact_agent": "contact_agent",
                "tracking_agent": "tracking_agent"
            }
        )
        
        return workflow.compile()
    await toolsRegistry.getTools(True,None, ctx)
    _tool = toolsRegistry.getToolByName("searchcontacts")
    print(_tool.name)
    return create_workflow()

agnet_name = "Miss_Daftary"
agnet_description="""
                An agent that can manage and tracke user activities such as 
                Managing tasks, Meetings, Phonecalls and emails.
                It can record activities in the company CRM application. 
                Usage Examples:
                    Register Phonecalls
                    Create/Send Emails 
                    Send SMS
                    Arrange/Register Meetings
                """
agent_system_prompt="""
                    You are a helpful assistant with access to various tools. 
                    Use them when needed to answer questions accurately. 
                    You work as an assistant in Hajir Sanat. 
                    Your primary task is to help users to manage their activities, specially:
                    1. Register activities such as registering phonecalls, meetings, emails.
                    2. Providing information about users activities, such as tasks.
                    You are provided with a set of tools for each of above tasks 
                    for example there is a tool to register a phone call, or send an sms.
                    Also note to register an activity it is often required to specify a client
                    or contact. For instance to register a phone call you need to specify the client 
                    who has called. In these cases you can use the specific tool to to find the contact.
                    """


class Agnet(BaseAgent):
    def __init__(self):
        super().__init__(agnet_name, agnet_description)
        self._llm:ChatOpenAI = None
    def ToMessage(self, msg:models.ChatMessage):
        return HumanMessage(msg.content[0].get('text')) if msg.role=='user' else AIMessage(msg.content[0].get('text'))

    async def get_llm(self) -> ChatOpenAI:
        if (self._llm == None):
            params: models.LLMParameters = (await self.get_llm_params())
            logger.info(f"LLM successfully initialize. Name:{params.Name}, Model:{params.Model}, Url:{params.Url}")
            self._llm = ChatOpenAI(model=params.Model, temperature=0,
                                   base_url=params.Url, api_key=params.ApiKey)
        return self._llm
    async def start(self):
        await super().start(bus)
        global llm 
        llm = await self.get_llm()
        
    async def reply(self, req: models.AgentRequest):
        logger.info(f"{self.name} starts.")
        _history = [self.ToMessage(msg)
                    for msg in req.chat_history]
        _history.append(HumanMessage(content=req.input_text))
        app = await createApp(req.context)
        initial_state = {
            "messages": _history,
            "current_user_id": req.context.UserId,
            "recent_contacts": [],
            "recent_activities": [],
            "next_agent": None
        }
        result = await app.ainvoke(initial_state)
        for message in result["messages"]:
            if isinstance(message, AIMessage):
                print(f"Assistant: {message.content}\n")

        return models.AgentResponse(text=result["messages"][-1].content)        
    
    def run_assistant(self,user_message: str, user_id: str = "USER_001"):
        """Run the assistant with a user message"""
        app = create_workflow()
        initial_state = {
            "messages": [HumanMessage(content=user_message)],
            "current_user_id": user_id,
            "recent_contacts": [],
            "recent_activities": [],
            "next_agent": None
        }
        
        print(f"\n{'='*70}")
        print(f"User: {user_message}")
        print(f"{'='*70}\n")
        
        result = app.invoke(initial_state)
        
    # Print the final response
        for message in result["messages"]:
            if isinstance(message, AIMessage):
                print(f"Assistant: {message.content}\n")




agent = Agnet()