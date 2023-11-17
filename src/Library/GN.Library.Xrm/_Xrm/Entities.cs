using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GN.Library.Xrm
{

    public class Entities
    {
        public enum Names
        {
            Account = 1,
            Contact = 2,
            Opportunity = 3,
            Lead = 4,
            Annotation = 5,
            BusinessUnitMap = 6,
            Owner = 7,
            SystemUser = 8,
            Team = 9,
            BusinessUnit = 10,
            PrincipalObjectAccess = 11,
            RolePrivileges = 12,
            SystemUserLicenses = 13,
            SystemUserPrincipals = 14,
            SystemUserRoles = 15,
            AccountLeads = 16,
            ContactInvoices = 17,
            ContactQuotes = 18,
            ContactOrders = 19,
            ServiceContractContacts = 20,
            ProductSalesLiterature = 21,
            ContactLeads = 22,
            TeamMembership = 23,
            LeadCompetitors = 24,
            OpportunityCompetitors = 25,
            CompetitorSalesLiterature = 26,
            LeadProduct = 27,
            RoleTemplatePrivileges = 28,
            Subscription = 29,
            FilterTemplate = 30,
            PrivilegeObjectTypeCodes = 31,
            SalesProcessInstance = 32,
            SubscriptionSyncInfo = 33,
            SubscriptionTrackingDeletedObject = 35,
            ClientUpdate = 36,
            SubscriptionManuallyTrackedObject = 37,
            TeamRoles = 40,
            PrincipalEntityMap = 41,
            SystemUserBusinessUnitEntityMap = 42,
            PrincipalAttributeAccessMap = 43,
            PrincipalObjectAttributeAccess = 44,
            PrincipalObjectAccessReadSnapshot = 90,
            RecordCountSnapshot = 91,
            Incident = 112,
            Competitor = 123,
            DocumentIndex = 126,
            KbArticle = 127,
            Subject = 129,
            BusinessUnitNewsArticle = 132,
            ActivityParty = 135,
            UserSettings = 150,
            ActivityMimeAttachment = 1001,
            Attachment = 1002,
            InternalAddress = 1003,
            CompetitorAddress = 1004,
            CompetitorProduct = 1006,
            Contract = 1010,
            ContractDetail = 1011,
            Discount = 1013,
            KbArticleTemplate = 1016,
            LeadAddress = 1017,
            Organization = 1019,
            OrganizationUI = 1021,
            PriceLevel = 1022,
            Privilege = 1023,
            Product = 1024,
            ProductAssociation = 1025,
            ProductPriceLevel = 1026,
            ProductSubstitute = 1028,
            SystemForm = 1030,
            UserForm = 1031,
            Role = 1036,
            RoleTemplate = 1037,
            SalesLiterature = 1038,
            SavedQuery = 1039,
            StringMap = 1043,
            UoM = 1055,
            UoMSchedule = 1056,
            SalesLiteratureItem = 1070,
            CustomerAddress = 1071,
            SubscriptionClients = 1072,
            StatusMap = 1075,
            DiscountType = 1080,
            KbArticleComment = 1082,
            OpportunityProduct = 1083,
            Quote = 1084,
            QuoteDetail = 1085,
            UserFiscalCalendar = 1086,
            SalesOrder = 1088,
            SalesOrderDetail = 1089,
            Invoice = 1090,
            InvoiceDetail = 1091,
            SavedQueryVisualization = 1111,
            UserQueryVisualization = 1112,
            RibbonTabToCommandMap = 1113,
            RibbonContextGroup = 1115,
            RibbonCommand = 1116,
            RibbonRule = 1117,
            RibbonCustomization = 1120,
            RibbonDiff = 1130,
            ReplicationBacklog = 1140,
            FieldSecurityProfile = 1200,
            FieldPermission = 1201,
            SystemUserProfiles = 1202,
            TeamProfiles = 1203,
            AnnualFiscalCalendar = 2000,
            SemiAnnualFiscalCalendar = 2001,
            QuarterlyFiscalCalendar = 2002,
            MonthlyFiscalCalendar = 2003,
            FixedMonthlyFiscalCalendar = 2004,
            Template = 2010,
            ContractTemplate = 2011,
            UnresolvedAddress = 2012,
            Territory = 2013,
            Queue = 2020,
            License = 2027,
            QueueItem = 2029,
            UserEntityUISettings = 2500,
            UserEntityInstanceData = 2501,
            IntegrationStatus = 3000,
            ConnectionRole = 3231,
            ConnectionRoleAssociation = 3232,
            ConnectionRoleObjectTypeCode = 3233,
            Connection = 3234,
            Equipment = 4000,
            Service = 4001,
            Resource = 4002,
            Calendar = 4003,
            CalendarRule = 4004,
            ResourceGroup = 4005,
            ResourceSpec = 4006,
            ConstraintBasedGroup = 4007,
            Site = 4009,
            ResourceGroupExpansion = 4010,
            InterProcessLock = 4011,
            EmailHash = 4023,
            DisplayStringMap = 4101,
            DisplayString = 4102,
            Notification = 4110,
            ActivityPointer = 4200,
            Appointment = 4201,
            Email = 4202,
            Fax = 4204,
            IncidentResolution = 4206,
            Letter = 4207,
            OpportunityClose = 4208,
            OrderClose = 4209,
            PhoneCall = 4210,
            QuoteClose = 4211,
            Task = 4212,
            ServiceAppointment = 4214,
            Commitment = 4215,
            UserQuery = 4230,
            RecurrenceRule = 4250,
            RecurringAppointmentMaster = 4251,
            EmailSearch = 4299,
            List = 4300,
            ListMember = 4301,
            Campaign = 4400,
            CampaignResponse = 4401,
            CampaignActivity = 4402,
            CampaignItem = 4403,
            CampaignActivityItem = 4404,
            BulkOperationLog = 4405,
            BulkOperation = 4406,
            Import = 4410,
            ImportMap = 4411,
            ImportFile = 4412,
            ImportData = 4413,
            DuplicateRule = 4414,
            DuplicateRecord = 4415,
            DuplicateRuleCondition = 4416,
            ColumnMapping = 4417,
            PickListMapping = 4418,
            LookUpMapping = 4419,
            OwnerMapping = 4420,
            ImportLog = 4423,
            BulkDeleteOperation = 4424,
            BulkDeleteFailure = 4425,
            TransformationMapping = 4426,
            TransformationParameterMapping = 4427,
            ImportEntityMapping = 4428,
            RelationshipRole = 4500,
            RelationshipRoleMap = 4501,
            CustomerRelationship = 4502,
            CustomerOpportunityRole = 4503,
            Audit = 4567,
            EntityMap = 4600,
            AttributeMap = 4601,
            PluginType = 4602,
            PluginTypeStatistic = 4603,
            PluginAssembly = 4605,
            SdkMessage = 4606,
            SdkMessageFilter = 4607,
            SdkMessageProcessingStep = 4608,
            SdkMessageRequest = 4609,
            SdkMessageResponse = 4610,
            SdkMessageResponseField = 4611,
            SdkMessagePair = 4613,
            SdkMessageRequestField = 4614,
            SdkMessageProcessingStepImage = 4615,
            SdkMessageProcessingStepSecureConfig = 4616,
            ServiceEndpoint = 4618,
            AsyncOperation = 4700,
            WorkflowWaitSubscription = 4702,
            Workflow = 4703,
            WorkflowDependency = 4704,
            IsvConfig = 4705,
            WorkflowLog = 4706,
            ApplicationFile = 4707,
            OrganizationStatistic = 4708,
            SiteMap = 4709,
            ProcessSession = 4710,
            WebWizard = 4800,
            WizardPage = 4802,
            WizardAccessPrivilege = 4803,
            TimeZoneDefinition = 4810,
            TimeZoneRule = 4811,
            TimeZoneLocalizedName = 4812,
            Solution = 7100,
            Publisher = 7101,
            PublisherAddress = 7102,
            SolutionComponent = 7103,
            Dependency = 7105,
            DependencyNode = 7106,
            InvalidDependency = 7107,
            Post = 8000,
            PostRole = 8001,
            PostRegarding = 8002,
            PostFollow = 8003,
            PostComment = 8005,
            PostLike = 8006,
            Report = 9100,
            ReportEntity = 9101,
            ReportCategory = 9102,
            ReportVisibility = 9103,
            ReportLink = 9104,
            TransactionCurrency = 9105,
            MailMergeTemplate = 9106,
            ImportJob = 9107,
            WebResource = 9333,
            SharePointSite = 9502,
            SharePointDocumentLocation = 9508,
            Goal = 9600,
            GoalRollupQuery = 9602,
            Metric = 9603,
            RollupField = 9604
        };

        public static string GetNameByCode(int code)
        {
            //var fff =Enum.GetNames(typeof(Names));
            return Enum.GetValues(typeof(Names))
                .Cast<int>()
                .FirstOrDefault(x => x == code) > 0
                    ? ((Names)code).ToString().ToLowerInvariant()
                    : null;
        }

        private static void TypeCodes()
        {
            var a = TYPECODES
                .Split('\r', '\n')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Split(' '))
                .Where(x => x.Length == 2)
                .Select(x => $"{x[0]}={x[1]},")
                .ToArray();
            var text = string.Join("\r\n", a);
        }
        private static string TYPECODES =
@"
Account 1
Contact 2
Opportunity 3
Lead 4
Annotation 5
BusinessUnitMap 6
Owner 7
SystemUser 8
Team 9
BusinessUnit 10
PrincipalObjectAccess 11
RolePrivileges 12
SystemUserLicenses 13
SystemUserPrincipals 14
SystemUserRoles 15
AccountLeads 16
ContactInvoices 17
ContactQuotes 18
ContactOrders 19
ServiceContractContacts 20
ProductSalesLiterature 21
ContactLeads 22
TeamMembership 23
LeadCompetitors 24
OpportunityCompetitors 25
CompetitorSalesLiterature 26
LeadProduct 27
RoleTemplatePrivileges 28
Subscription 29
FilterTemplate 30
PrivilegeObjectTypeCodes 31
SalesProcessInstance 32
SubscriptionSyncInfo 33
SubscriptionTrackingDeletedObject 35
ClientUpdate 36
SubscriptionManuallyTrackedObject 37
TeamRoles 40
PrincipalEntityMap 41
SystemUserBusinessUnitEntityMap 42
PrincipalAttributeAccessMap 43
PrincipalObjectAttributeAccess 44
PrincipalObjectAccessReadSnapshot 90
RecordCountSnapshot 91
Incident 112
Competitor 123
DocumentIndex 126
KbArticle 127
Subject 129
BusinessUnitNewsArticle 132
ActivityParty 135
UserSettings 150
ActivityMimeAttachment 1001
Attachment 1002
InternalAddress 1003
CompetitorAddress 1004
CompetitorProduct 1006
Contract 1010
ContractDetail 1011
Discount 1013
KbArticleTemplate 1016
LeadAddress 1017
Organization 1019
OrganizationUI 1021
PriceLevel 1022
Privilege 1023
Product 1024
ProductAssociation 1025
ProductPriceLevel 1026
ProductSubstitute 1028
SystemForm 1030
UserForm 1031
Role 1036
RoleTemplate 1037
SalesLiterature 1038
SavedQuery 1039
StringMap 1043
UoM 1055
UoMSchedule 1056
SalesLiteratureItem 1070
CustomerAddress 1071
SubscriptionClients 1072
StatusMap 1075
DiscountType 1080
KbArticleComment 1082
OpportunityProduct 1083
Quote 1084
QuoteDetail 1085
UserFiscalCalendar 1086
SalesOrder 1088
SalesOrderDetail 1089
Invoice 1090
InvoiceDetail 1091
SavedQueryVisualization 1111
UserQueryVisualization 1112
RibbonTabToCommandMap 1113
RibbonContextGroup 1115
RibbonCommand 1116
RibbonRule 1117
RibbonCustomization 1120
RibbonDiff 1130
ReplicationBacklog 1140
FieldSecurityProfile 1200
FieldPermission 1201
SystemUserProfiles 1202
TeamProfiles 1203
AnnualFiscalCalendar 2000
SemiAnnualFiscalCalendar 2001
QuarterlyFiscalCalendar 2002
MonthlyFiscalCalendar 2003
FixedMonthlyFiscalCalendar 2004
Template 2010
ContractTemplate 2011
UnresolvedAddress 2012
Territory 2013
Queue 2020
License 2027
QueueItem 2029
UserEntityUISettings 2500
UserEntityInstanceData 2501
IntegrationStatus 3000
ConnectionRole 3231
ConnectionRoleAssociation 3232
ConnectionRoleObjectTypeCode 3233
Connection 3234
Equipment 4000
Service 4001
Resource 4002
Calendar 4003
CalendarRule 4004
ResourceGroup 4005
ResourceSpec 4006
ConstraintBasedGroup 4007
Site 4009
ResourceGroupExpansion 4010
InterProcessLock 4011
EmailHash 4023
DisplayStringMap 4101
DisplayString 4102
Notification 4110
ActivityPointer 4200
Appointment 4201
Email 4202
Fax 4204
IncidentResolution 4206
Letter 4207
OpportunityClose 4208
OrderClose 4209
PhoneCall 4210
QuoteClose 4211
Task 4212
ServiceAppointment 4214
Commitment 4215
UserQuery 4230
RecurrenceRule 4250
RecurringAppointmentMaster 4251
EmailSearch 4299
List 4300
ListMember 4301
Campaign 4400
CampaignResponse 4401
CampaignActivity 4402
CampaignItem 4403
CampaignActivityItem 4404
BulkOperationLog 4405
BulkOperation 4406
Import 4410
ImportMap 4411
ImportFile 4412
ImportData 4413
DuplicateRule 4414
DuplicateRecord 4415
DuplicateRuleCondition 4416
ColumnMapping 4417
PickListMapping 4418
LookUpMapping 4419
OwnerMapping 4420
ImportLog 4423
BulkDeleteOperation 4424
BulkDeleteFailure 4425
TransformationMapping 4426
TransformationParameterMapping 4427
ImportEntityMapping 4428
RelationshipRole 4500
RelationshipRoleMap 4501
CustomerRelationship 4502
CustomerOpportunityRole 4503
Audit 4567
EntityMap 4600
AttributeMap 4601
PluginType 4602
PluginTypeStatistic 4603
PluginAssembly 4605
SdkMessage 4606
SdkMessageFilter 4607
SdkMessageProcessingStep 4608
SdkMessageRequest 4609
SdkMessageResponse 4610
SdkMessageResponseField 4611
SdkMessagePair 4613
SdkMessageRequestField 4614
SdkMessageProcessingStepImage 4615
SdkMessageProcessingStepSecureConfig 4616
ServiceEndpoint 4618
AsyncOperation 4700
WorkflowWaitSubscription 4702
Workflow 4703
WorkflowDependency 4704
IsvConfig 4705
WorkflowLog 4706
ApplicationFile 4707
OrganizationStatistic 4708
SiteMap 4709
ProcessSession 4710
WebWizard 4800
WizardPage 4802
WizardAccessPrivilege 4803
TimeZoneDefinition 4810
TimeZoneRule 4811
TimeZoneLocalizedName 4812
Solution 7100
Publisher 7101
PublisherAddress 7102
SolutionComponent 7103
Dependency 7105
DependencyNode 7106
InvalidDependency 7107
Post 8000
PostRole 8001
PostRegarding 8002
PostFollow 8003
PostComment 8005
PostLike 8006
Report 9100
ReportEntity 9101
ReportCategory 9102
ReportVisibility 9103
ReportLink 9104
TransactionCurrency 9105
MailMergeTemplate 9106
ImportJob 9107
WebResource 9333
SharePointSite 9502
SharePointDocumentLocation 9508
Goal 9600
GoalRollupQuery 9602
Metric 9603
RollupField 9604";
    }
}

