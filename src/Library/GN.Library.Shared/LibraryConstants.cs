using GN.Library.Shared.ServiceDiscovery;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml;

namespace GN.Library
{

    public class LibraryConstants
    {
        public const int DefaultTimeout = 15000;
        public static bool IsNetCore;
        public static string DomianName = "gnco.local";
        public const string PUSH_NOTIFICATION_API_ROUTE = "push-notifications-api";
        public static string SignalRTransportReceiveMethod => "Receive";
        public static string SignalRSubscribeMethod => "Subscribe";
        public static string SignalRUserSubscribeMethod => "SubscribeUser";

        public const string SignalRClientReceiveMethodName = "ReceivePhoneCall";
        public static string PushNotificationApiRoute => PUSH_NOTIFICATION_API_ROUTE;
        public const string SipRawTopic = "SIP/RAW";

        public const string MessageBusHubUrl = "/eventhub";
        public class Secret
        {
            public string SecretKey = "secret";
            public string SecrentAccount = "taminwan.tent\\bmsd_admin";
        }
        public class ChatHubMethodNames
        {
            public const string Authorize = "Authorize";
            public const string SignIn = "SignIn";
            public const string Apply = "Apply";
            public const string PostMessage = "PostMessage";
            public const string Search = "Search";
            public const string Subscribe = "Subscribe";
            public const string Execute_deprecated = "Execute_deprecated";
            public const string ExecuteCommand = "ExecuteCommand";
        }
        public class MessagingConstants
        {
            public const int SIGNALR_MAX_BUFFER_SIZE = 512 * 1024;
            public class HeaderKeys
            {
                public const string StreamName = "$event-stream";
                public const string StreamId = "$event-stream";
                public const string Topic = "$event_topic";
                public const string Vesion = "$event-version";
                public const string From = "#from_endpoint";
                public const string To = "#to_endpoint";
                public const string InReplyTo = "#in_reply_to";
                public const string VisitedEndpoints = "#visited_endpoints";
                public const string StatusCode = "#statuscode";
                public const string ErrorMessage = "#errormessage";
            }
            public class Topics
            {
                public const string SaveEvent = "$new";
                public const string OpenStream = "$open-stream";
                public const string ReplayStream = "$replay-stream";
                public const string Reply = "$reply";
                public const string Acquire = "$acquire";
            }
        }
        public class ServiceNames : GN.Library.Shared.ServiceDiscovery.ServiceNames
        {

        }
        public class Subjects
        {
            private const string Library = "Library";
            public const string StarStar = "**";

            public class Messaging
            {
                public const string CreateQueue = Library + ".createqueue";
                public const string QueueMessage = Library + ".queue-message";
            }
            public class IdentityServices
            {
                public const string LoadUsers = Library + ".loadusers";
                public const string UserSignedIn = Library + ".usersignin";
                public const string UserDisconnected = Library + ".userdisconnected";
                public const string AuthenticateUser = Library + ".authenticateuser";
                public const string QueryUser = Library + ".queryuser";
            }
            public class EnityServices
            {
                private const string EntityServices = Library + ".EntityServices";
                public const string Watch = EntityServices + ".Watch";
                public const string Upsert = EntityServices + ".Upsert";
                public const string Get = EntityServices + ".Get";
                public const string Delete = EntityServices + ".Delete";
                public const string Unwatch = EntityServices + ".Unwatch";
                private const string EntityUpdatedEvent = EntityServices + ".updated.";
                private const string EntityDeletedEvent = EntityServices + ".deleted.";
                public static string GetEntityUpdatedEvent(string logicalName) => EntityUpdatedEvent + logicalName;
                public static string GetEntityDeletedEvent(string logicalName) => EntityDeletedEvent + logicalName;
                public const string AnyEntityUpdatedEvent = EntityUpdatedEvent + "*";
            }
            public class OfficeAssistant
            {
                public const string OfficeAssistantPerfix = "assistant";
                public const string MessageReceived = OfficeAssistantPerfix + ".received";
                public const string SendMessage = OfficeAssistantPerfix + ".send";
                public const string ExecuteCommandLine = OfficeAssistantPerfix + ".execute";


            }
            public class Nodtifications
            {
                public static string NotificationsPerfix => "notifications";

                /// <summary>
                /// removes '.' from userid. "babak@gnco.local" => babak@gnco!local" to be used in subjects.
                /// </summary>
                /// <param name="userName"></param>
                /// <returns></returns>
                public static string GetNormalizedUserNameForSubjects(string userName)
                {
                    return userName?.ToLowerInvariant();
                }
                public static string GetNotificationTopic(string userName, string topic = "message")
                {
                    return $"{NotificationsPerfix}.{GetNormalizedUserNameForSubjects(userName)}.{topic}";
                }

            }
            public class ServiceDiscovery : ServiceDiscoverySubjects
            {

            }
            public class UserRequests
            {
                public const string Perfix = "userrequests";
                public static string WhoAmI => Perfix + ".whoami";
                public static string Search => Perfix + ".search";
                public static string SearchResult => Perfix + ".searchresult";
                public static string FetchPosts => Perfix + ".fetchposts";
                public const string StartMyUpdates = Perfix + ".my.start";
                public static string MyUpdate => Perfix + ".my.update";
                public const string GetUpdate = Perfix + ".my.get";
            }
        }

        public class Schema
        {

            public class EntitySchema
            {
                public const string StateCode = "statecode";
                public const string StatusCode = "statuscode";
                public const string CreatedOn = "createdon";
                public const string ModifiedOn = "modifiedon";
                public const string CreatedBy = "createdby";
                public const string ModifiedBy = "modifiedby";
                public const string Owner = "ownerid";
                public const string OwningUser = "owninguser";
                public const string OwningTeam = "owningteam";
                public const string OwningBuisnessUnit = "owningbusinessunit";
                public class WhereClauses
                {
                }
            }

            public class Account : EntitySchema
            {

                public const string LogicalName = "account";
                public const string AccountId = "accountid";
                public const string Name = "name";
                public const string Telephone1 = "telephone1";
                public const string Telephone2 = "telephone1";
                public const string Telephone3 = "telephone1";
                public const string Fax = "fax";
                public const string Address1_Line1 = "address1_line1";
                public const string Address1_Line2 = "address1_line2";
                public const string Address1_Line3 = "address1_line2";
                public const string Address1_Fax = "address1_fax";
                public const string Address1_City = "address1_city";
                public const string address1_stateorprovince = "address1_stateorprovince";
                public const string address1_country = "address1_country";

                public const string Address1_Telehone1 = "address1_telephone1";
                public const string Address1_Telehone2 = "address1_telephone2";
                public const string Address1_Telehone3 = "address1_telephone3";
                public const string WebSiteUrl = "websiteurl";
                public const string Email1 = "emailaddress1";
                public const string AccountNumber = "accountnumber";
                public const string PrimaryContactId = "primarycontactid";
                public const string ParentAccount = "parentaccountid";
                public const string AccountRatingCode = "accountratingcode";


               

            }


        }
    }
}
