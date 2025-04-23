using Microsoft.Win32;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Hajir.Crm.Sales.Xrm.Plugins
{

    public interface IJsonSerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string text);
        object Deserialize(string text, Type type);
    }
    public class JavaScriptSerializerEx : IJsonSerializer
    {
        private JavaScriptSerializer serializer;

        private JavaScriptSerializer GetInstance()
        {

            this.serializer = this.serializer ?? new JavaScriptSerializer();
            return this.serializer;
        }
        public T Deserialize<T>(string text)
        {
            return GetInstance().Deserialize<T>(text);
        }

        public string Serialize(object obj)
        {
            return GetInstance().Serialize(obj);
        }

        public object Deserialize(string text, Type type)
        {
            return this.GetInstance().Deserialize(text, type);
        }
    }

    public class PluginSerializer
    {

        public static IJsonSerializer GetSerializer()
        {
            return new JavaScriptSerializerEx();
        }

    }
    public class PluginException : Exception
    {
        public PluginException(string message) : base(message) { }
    }
    public class PluginNetworkException : PluginException
    {
        public PluginNetworkException(string message) : base(message) { }
    }
    public class PluginValidationException : PluginException
    {
        public PluginValidationException(string message) : base(message) { }
    }

    public class PluginLogger
    {
        private StringBuilder stringLog;
        private ITracingService tracingService;
        public IOrganizationService OrganizationService { get; set; }
        public string GetLog() => stringLog.ToString();

        public PluginLogger(ITracingService tracingService, IOrganizationService service)
        {
            this.tracingService = tracingService;
            this.stringLog = new StringBuilder();
            this.OrganizationService = service;
        }

        public void Log(string fmt, params object[] args)
        {
            this.tracingService?.Trace(fmt, args);
            this.stringLog.AppendLine(string.Format(fmt, args));
        }
        public void Flush()
        {
            var e = new Entity("task");
            try
            {
                e["subject"] = "log";
                e["description"] = this.stringLog.ToString();
                Guid? id = OrganizationService?.Create(e);
                if (id != null)
                    this.stringLog = new StringBuilder();
            }
            catch { }
        }
    }
    public class WebCommandHelper
    {
        public enum CommandStatus
        {
            Error = -10,
            Failed = -5,
            NotStared = -1,
            InProgress = 0,
            Success = 1,
        }
        public class WebCommandRequest
        {
            public string Request { get; set; }
            public string Data { get; set; }
            public Guid? CurrentUserId { get; set; }
            public bool UseAdminAccount { get; set; }
            public string CurrentUserName { get; set; }
            public string CurrentUserKey { get; set; }
            public string DeviceId { get; set; }
            public override string ToString()
            {
                return string.Format("Command:'{0}', CurrentUserId:'{1}', UseAdminAccount:'{2}'", Request, CurrentUserId, UseAdminAccount);
            }
        }
        public class WebCommandResponse
        {
            public CommandStatus Status { get; set; }
            public string Data { get; set; }
            public string Message { get; set; }
            public override string ToString()
            {
                return string.Format("Success:{0}, Message:{1}, Data:{2}",
                    Status,
                    Message,
                    string.IsNullOrWhiteSpace(Data) ? "NULL" :
                    Data.Length > 30 ? Data.Substring(0, 30) + "..." : Data
                    );
            }
        }
        public async Task<TRes> SendCommand<TReq, TRes>(string url, string commandName, TReq req)
        where TReq : class, new()
        where TRes : class, new()
        {
            TRes result = null;
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var serializer = PluginHelper.GetSerializer();// new JavaScriptSerializer();
            try
            {
                var _req = new WebCommandRequest();
                _req.Request = commandName;
                _req.Data = serializer.Serialize(req);
                var content = new StringContent(serializer.Serialize(_req), Encoding.UTF8, "application/json");

                var resp = await client.PostAsync("api/xrmapi2", content).ConfigureAwait(false);
                if (resp.IsSuccessStatusCode && resp.Content != null)
                {
                    var strResponse = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var _resp = serializer.Deserialize<WebCommandResponse>(strResponse);
                    result = serializer.Deserialize<TRes>(_resp.Data);

                }
                else
                    throw new Exception("Unable to call WebAPI");
            }
            catch (Exception err)
            {
                throw new PluginNetworkException($"Failed to call WebAPI, Url:{url}, Err:{err.GetBaseException().Message}");
            }
            return result;
        }
    }
    public class EntityWebCommand
    {
        public Task<EntityWebCommandResponseModel> Send(string url, EntityWebCommandRequestModel req)
        {
            var helper = new WebCommandHelper();
            return helper.SendCommand<EntityWebCommandRequestModel, EntityWebCommandResponseModel>(url, typeof(EntityWebCommand).Name, req);
        }
        public Task<EntityWebCommandResponseModel> Send(string url, Action<EntityWebCommandRequestModel> config)
        {
            var request = new EntityWebCommandRequestModel();
            config?.Invoke(request);
            return Send(url, request);
        }
    }
    public class MessagePumpHelper
    {
        const string REG_KEY = @"Software\Gostareh Negar\Dynamic\XrmPlugin";
        const string REG_URL = "Url";
        const string REG_OnUpdate = "On_Update";
        const string REG_OnUpdateSync = "On_Update_Synch";
        public string WebApiBaseUrl { get; private set; }
        public string OnUpdate { get; private set; }
        public string OnUpdateSynch { get; private set; }
        private PluginLogger logger;
        public string OrganizationName { get; set; }
        public string ServerMachineName { get; set; }

        public MessagePumpHelper() : this(null, "default", "")
        {
        }
        public MessagePumpHelper(ITracingService tracing, string organizationName, string serverMacineName)
        {
            this.logger = new PluginLogger(tracing, null);
            this.OrganizationName = organizationName;
            this.ServerMachineName = serverMacineName;
            Refresh();

        }
        public MessagePumpHelper Refresh()
        {
            var is32 = IntPtr.Size == 4;
            var reg = this.GetRegistryKey(false);
            if (reg != null)
            {
                this.WebApiBaseUrl = reg.GetValue(REG_URL, "").ToString();
                this.OnUpdate = reg.GetValue(REG_OnUpdate, "").ToString();
                this.OnUpdateSynch = reg.GetValue(REG_OnUpdateSync, "").ToString();
            }
            return this;
        }
        private RegistryKey GetRegistryKey(bool writable)
        {
            RegistryKey baseKey = null;
            RegistryKey result = null;
            var path = REG_KEY + @"\" + this.OrganizationName;
            baseKey = RegistryKey
                        .OpenRemoteBaseKey(RegistryHive.LocalMachine, this.ServerMachineName, RegistryView.Registry64);
            if (baseKey != null)
            {
                result = baseKey.OpenSubKey(path, writable);
                if (result == null)
                    try { result = baseKey.CreateSubKey(path); } catch { }
            }
            return result;
        }
        private bool SetValue(string name, string value)
        {
            try
            {
                var reg = this.GetRegistryKey(true);
                reg?.SetValue(name, value);
                return reg?.GetValue(name).ToString() == value;
            }
            catch { }
            return false;


        }
        public bool SetUrl(string url)
        {
            return SetValue(REG_URL, url);
        }

        private List<string> ToList(string str)
        {
            return str.Trim().ToLowerInvariant()
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }
        private string ListToString(IEnumerable<string> list)
        {
            var result = "";
            foreach (var item in list)
            {
                result += item + ";";
            }
            return result.EndsWith(";")
                ? result.Substring(0, result.Length - 1)
                : result;
        }
        public IEnumerable<string> GetOnUpdateList(bool refresh = false)
        {
            if (refresh)
                Refresh();
            return ToList(this.OnUpdate);
        }
        public bool AddOnUpdate(string logicalName, bool refresh = false)
        {
            if (!string.IsNullOrWhiteSpace(logicalName))
            {
                logicalName = logicalName.ToLowerInvariant();
                var currentValues = this.GetOnUpdateList(refresh).ToList();// ToList(this.Refresh(refresh).OnUpdate);
                if (!currentValues.Contains(logicalName))
                {
                    currentValues.Add(logicalName);
                    SetValue(REG_OnUpdate, ListToString(currentValues));
                }
            }
            return this.GetOnUpdateList(true).Contains(logicalName);
        }
        public bool RemoveOnUpdate(string logicalName, bool refersh = false)
        {
            if (!string.IsNullOrWhiteSpace(logicalName))
            {
                logicalName = logicalName.ToLowerInvariant();
                var current = this.GetOnUpdateList(refersh)
                    .Where(x => x != logicalName).ToList();
                SetValue(REG_OnUpdate, ListToString(current));
            }
            return !this.GetOnUpdateList(true).Contains(logicalName);
        }
        public bool ClearOnUpdate()
        {
            return SetValue(REG_OnUpdate, "");
        }
        public IEnumerable<string> GetOnUpdateSyncList(bool refresh = false)
        {
            if (refresh)
                Refresh();
            return ToList(this.OnUpdateSynch);
        }
        public bool AddOnUpdateSync(string logicalName, bool refresh = false)
        {
            if (!string.IsNullOrWhiteSpace(logicalName))
            {
                logicalName = logicalName.ToLowerInvariant();
                var currentValues = this.GetOnUpdateSyncList(refresh).ToList();
                if (!currentValues.Contains(logicalName))
                {
                    currentValues.Add(logicalName);
                    SetValue(REG_OnUpdateSync, ListToString(currentValues));
                }
            }
            return this.GetOnUpdateSyncList(true).Contains(logicalName);
        }
        public bool RemoveOnUpdateSync(string logicalName, bool refersh = false)
        {
            if (!string.IsNullOrWhiteSpace(logicalName))
            {
                logicalName = logicalName.ToLowerInvariant();
                var current = this.GetOnUpdateSyncList(refersh)
                    .Where(x => x != logicalName).ToList();
                SetValue(REG_OnUpdateSync, ListToString(current));
            }
            return !this.GetOnUpdateSyncList(true).Contains(logicalName);
        }
        public bool ClearOnUpdateSync()
        {
            return SetValue(REG_OnUpdateSync, "");
        }

        public string Send(Entity entity, Entity PreImage, Entity PostImage)
        {
            var result = "";
            var url = this.WebApiBaseUrl;
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                if (entity != null && entity.LogicalName != null &&
                    this.GetOnUpdateList().Contains(entity.LogicalName.ToLowerInvariant()))
                {
                    Task.Run(async () =>
                    {
                        var command = new EntityWebCommand();
                        var response = await command.Send(url, cfg =>
                        {
                            cfg.Entity = new JsonSerializableEntity(entity);
                            cfg.PostImage = new JsonSerializableEntity(PostImage);
                            cfg.PreImage = new JsonSerializableEntity(PreImage);
                            cfg.MessageName = "onupdate";
                        }).ConfigureAwait(false);
                    });
                }
                if (entity != null && entity.LogicalName != null &&
                    this.GetOnUpdateSyncList().Contains(entity.LogicalName.ToLowerInvariant()))
                {
                    var command = new EntityWebCommand();
                    var task = command.Send(url, cfg =>
                    {
                        cfg.Entity = new JsonSerializableEntity(entity);
                        cfg.PostImage = new JsonSerializableEntity(PostImage);
                        cfg.PreImage = new JsonSerializableEntity(PreImage);
                        cfg.MessageName = "onupdate";
                    });//.ConfigureAwait(false).GetAwaiter().GetResult();
                    task.ConfigureAwait(false);
                    if (task.Wait(1 * 60 * 1000) && task.IsCompleted)
                    {
                        var response = task.Result;
                        result = response.Error;
                    }
                    else
                    {
                        logger.Log(
                            "Timout occured while trying to pump message. Url:{0}", url);
                    }
                }

            }
            return result;
        }
    }

    public class PluginHelper
    {
        private readonly IServiceProvider serviceProvider;
        private StringBuilder stringLog;
        public PluginConfiguration PluginConfiguration { get; private set; }
        private Guid Id;
        public IPlugin Plugin { get; private set; }
        public IOrganizationServiceFactory GetOrganizationServiceFactory()
        {
            return (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        }
        public IOrganizationService GetOrganizationService()
        {
            return this.GetOrganizationServiceFactory().CreateOrganizationService(null);
        }
        public IPluginExecutionContext GetPluginExecutionContext()
        {
            return (IPluginExecutionContext)
                    serviceProvider.GetService(typeof(IPluginExecutionContext));
        }
        public PluginHelper(IPlugin plugin, IServiceProvider serviceProvider, string config, Guid instanceId)
        {
            this.serviceProvider = serviceProvider;
            this.stringLog = new StringBuilder();
            this.Id = new Guid();
            this.PluginConfiguration = PluginConfiguration.Desrialize(config);
            this.Id = instanceId;
            this.Plugin = plugin;
        }
        public void Log(string format, params object[] args)
        {
            try
            {
                this.stringLog.AppendLine(string.Format(format, args));
            }
            catch { }
        }
        public void FlushLogs()
        {
            if (this.PluginConfiguration.Trace)
            {
                var e = new Entity("task");
                var log = this.stringLog.ToString();
                try
                {
                    e["subject"] = string.Format("PluginLog ({0}) {1} {2}",
                        this.GetType().Assembly.GetName().Version,
                        DateTime.Now.ToShortDateString(),
                        DateTime.Now.ToLongTimeString());
                    e["description"] = this.stringLog.ToString();
                    Guid? id = GetOrganizationService()?.Create(e);
                    if (id != null)
                        this.stringLog = new StringBuilder();
                }
                catch { }
                if (this.PluginConfiguration.TraceThrow)
                    throw new Exception(log);

            }
        }
        public class _EntityRef
        {
            public string LogicalName { get; set; }
            public Guid Id { get; set; }

        }
        public void ApplyChanges(Entity entity, List<ChangeValue> changes, IJsonSerializer serializer)
        {
            var ser = serializer ?? PluginHelper.GetSerializer();
            //return;
            foreach (var change in changes)
            {
                try
                {
                    Type type = null;
                    try { type = Type.GetType(change.Type); } catch {  }
                    object val = null;
                    if (change.Value != null && type != null)
                    {
                        try
                        {
                            val = ser.Deserialize(change.Value, type);
                        }
                        catch {  }
                    }
                    if (type == typeof(EntityReference))
                    {
                        try
                        {
                            var e = ser.Deserialize<_EntityRef>(change.Value);
                            val = new EntityReference(e.LogicalName, e.Id);

                        }
                        catch { }
                    }
                    if (change.Key == "statuscode" && type == typeof(Int32))
                    {
                        try
                        {
                            var _val = ser.Deserialize<int>(change.Value);
                            val = new OptionSetValue(_val);
                        }
                        catch { }

                    }
                    entity[change.Key] = val;
                    Log("Change Key:{0}, Type:{1}, Value:{2}", change.Key, change.Type, val);
                }
                catch (Exception err)
                {
                    throw new PluginValidationException(string.Format(
                        "An error occured while trying to apply changes. Key:{0}, Value:{1}, Error:{2}, Type:{3}", change.Key, change.Value, err.Message, change.Type));
                }
            }


        }
        public void SendMessage(bool synch, Entity entity, Entity preImage, Entity postImage)
        {
            var url = this.PluginConfiguration.WebApiUrl;
            var command = new EntityWebCommand();
            var context = GetPluginExecutionContext();
            synch = synch || this.PluginConfiguration.SendSynch;
            Log("Send Message Begins: Plugin: '{0}', Version: '{1}', IsSync: '{2}', Organization: '{3}' " +
                "RequestId: '{4}', Stage:{5}",
                this.Plugin?.GetType().Name,
                this.Plugin?.GetType().Assembly.GetName().Version,
                synch,
                context?.OrganizationName,
                context?.RequestId, context?.Stage);
            Log("Entity: Name: '{0}' Id:'{1}'", entity?.LogicalName, entity?.Id);
            Log("PreImage: Name: '{0}' Id:'{1}'", preImage?.LogicalName, preImage?.Id);
            Log("PostImage: {0}", postImage?.Id, postImage?.LogicalName);
            Log("Url:{0}", url);
            //context.OrganizationName
            var task = command.Send(url, new EntityWebCommandRequestModel
            {
                Entity = entity == null ? null : new JsonSerializableEntity(entity),
                PostImage = postImage == null ? null : new JsonSerializableEntity(postImage),
                PreImage = preImage == null ? null : new JsonSerializableEntity(preImage),
                PrimaryEntityId = context?.PrimaryEntityId ?? Guid.Empty,
                PrimaryEtityLogicalName = context?.PrimaryEntityName,
                MessageName = context.MessageName,
                Mode = context.Mode,
                Stage = context.Stage,
                IsSynchronous = synch,
                PluginName = this.Plugin?.GetType().Name,
                RequestId = context?.RequestId ?? Guid.Empty,
                InitiatingUserId = context?.InitiatingUserId ?? Guid.Empty,
                OrganizationName = context?.OrganizationName,
                BuisnessUnitIdOfCallingUser = context?.BusinessUnitId ?? Guid.Empty,

                ProcessingStepId = context != null && context.OwningExtension != null &&
                context?.OwningExtension?.LogicalName == "sdkmessageprocessingstep"
                        ? context.OwningExtension.Id
                        : Guid.Empty
            });//.ConfigureAwait(false);
            task.ConfigureAwait(false);
            if (synch)
            {
                var timeOut = this.PluginConfiguration.TimeOut < 10 ? 30 * 10 * 1000 : this.PluginConfiguration.TimeOut;
                Log("This is a sync call. We will wait {0} milliseconds for results.", timeOut);
                try { task.Wait(timeOut); }
                catch
                {

                }
                if (task.IsFaulted && task.Exception != null)
                {
                    throw task.Exception;
                }
                else if (!task.IsCompleted)
                {
                    throw new PluginNetworkException("TimeOut");
                }
                else
                {
                    var result = task.Result;
                    if (result == null)
                        throw new PluginNetworkException("Api Result is Unexpectedly NULL.");
                    Log("Api Successfully Called. Message:{0}", result.Error);
                    if (!string.IsNullOrWhiteSpace(result.Error))
                        throw new PluginValidationException(result.Error);
                    if (result.Changes != null)
                    {
                        Log("Result has changes");
                        var ser = PluginHelper.GetSerializer();
                        this.ApplyChanges(entity, result.Changes, ser);
                        //foreach (var change in result.Changes)
                        //{
                        //	try
                        //	{
                        //		var type = Type.GetType(change.Type);
                        //		var val = ser.Deserialize(change.Value, type);
                        //		entity[change.Key] = ser.Deserialize(change.Value, type);
                        //		Log("Change Key:{0}, Type:{1}, Value:{2}", change.Key, change.Type, val);
                        //	}
                        //	catch (Exception err)
                        //	{
                        //		throw new PluginValidationException(string.Format(
                        //			"An error occured while trying to apply changes. Key:{0}, Value:{1}, Error:{2}", change.Key, change.Value, err.Message));
                        //	}
                        //}


                        ////var ffff = Newtonsoft.Json.JsonConvert.DeserializeObject<ChangeValue[]>(result.Data);
                        //var ffff = result.Changes.ToArray();
                        //Log("***L:{0}", ffff.Length);
                        //foreach (var v in ffff)
                        //{
                        //    entity[v.Key] = v.Value;
                        //    Log("***Change:{0} to  Value:{1}", v.Key, v.Value);
                        //}


                        //Log("Data: {0}", result.Data);
                        //throw new PluginValidationException("trace LOG");
                        //var e = result.Enity.GetEntity();
                        //foreach (var attrib in e.Attributes)
                        //{
                        //    entity[attrib.Key] = attrib.Value;
                        //}
                        //throw new PluginValidationException("trace....");
                    }
                }
            }
        }
        public void Publish(Entity entity, Entity preImage, Entity postImage)
        {
            SendMessage(false, entity, postImage: postImage, preImage: preImage);

        }
        public void Send(Entity entity, Entity preImage, Entity postImage)
        {
            SendMessage(true, entity, preImage, postImage);
        }
        public bool Handle(Exception err)
        {
            Log("An error occured while trying to send message: {0}.{1}.", err.Message, err.InnerException?.Message);
            return this.PluginConfiguration == null || !this.PluginConfiguration.IsCritical;
        }
        public string GetLog()
        {
            return this.stringLog?.ToString();
        }

        public static IJsonSerializer GetSerializer()
        {
            return new JavaScriptSerializerEx();
        }
    }
    public class XrmMessageBusPlugin : IPlugin
    {
        private string configString;
        private string cfg2;
        private PluginConfiguration cofiguration;
        private Guid InstanceId = Guid.NewGuid();
        public XrmMessageBusPlugin(string unsecureString, string secureString)
        {
            this.configString = unsecureString;
            this.cfg2 = secureString;
            this.cofiguration = PluginConfiguration.Desrialize(this.configString);
            try
            {
                this.cofiguration = new JavaScriptSerializer().Deserialize<PluginConfiguration>(this.configString);
            }
            catch { }
        }
        public XrmMessageBusPlugin()
        {

        }
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            PluginHelper helper = null;
            try
            {
                helper = new PluginHelper(this, serviceProvider, this.configString, this.InstanceId);
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)
                    serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.IsExecutingOffline || context.IsOfflinePlayback)
                    return;
                Entity entity = context.InputParameters.Contains("Target") ? context.InputParameters["Target"] as Entity : null;
                Entity pre_image = context.PreEntityImages.Contains("Target") ? context.PreEntityImages["Target"] as Entity : null;
                Entity post_image = context.PostEntityImages.Contains("Target") ? context.PostEntityImages["Target"] as Entity : null;
                helper.Publish(entity, pre_image, post_image);
            }
            catch (Exception err)
            {
                tracingService.Trace("An error occured while trying to pump message. Error:{0}", err.GetBaseException().Message);
                if (!helper.Handle(err))
                {
                    if (helper?.PluginConfiguration.Trace ?? false)
                        throw new Exception(err.Message + "\r\n" + "Log:" + helper?.GetLog());
                    else
                        throw;
                }
            }
            helper?.FlushLogs();

        }
    }
    public class TestPlugin : IPlugin
    {
        private string configString;
        private string cfg2;
        private PluginConfiguration cofiguration;
        private Guid InstanceId = Guid.NewGuid();
        public TestPlugin(string unsecureString, string secureString)
        {
            this.configString = unsecureString;
            this.cfg2 = secureString;
            this.cofiguration = PluginConfiguration.Desrialize(this.configString);
            try
            {
                this.cofiguration = new JavaScriptSerializer().Deserialize<PluginConfiguration>(this.configString);
            }
            catch { }
        }
        public TestPlugin()
        {

        }
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            var log = new StringBuilder();
            try
            {
                log.AppendLine("TestPlugin Starts...");
                log.AppendLine(string.Format("PrimaryEntity {0}, {1}", context.PrimaryEntityId, context.PrimaryEntityName));
                Entity entity = context.InputParameters.Contains("Target") ? context.InputParameters["Target"] as Entity : null;
                Entity pre_image = context.PreEntityImages.Contains("Target") ? context.PreEntityImages["Target"] as Entity : null;
                Entity post_image = context.PostEntityImages.Contains("Target") ? context.PostEntityImages["Target"] as Entity : null;
                log.AppendLine(string.Format("Target: {0} - {1} ", entity?.LogicalName, entity?.Id));
                log.AppendLine(string.Format("Pre_Image: {0} - {1} ", pre_image?.LogicalName, pre_image?.Id));
                log.AppendLine(string.Format("Pre_Image: {0} - {1} ", post_image?.LogicalName, post_image?.Id));

                log.AppendLine(string.Format(
                    "OwingContext:{0} - {1}", context.OwningExtension.LogicalName, context.OwningExtension.Name));
            }
            catch (Exception err)
            {
                log.AppendLine(string.Format("Error:", err));
            }

            if (this.cofiguration.TraceThrow)
            {
                throw new Exception(log.ToString());
            }
        }
    }


    public class ValidationPlugin : IPlugin
    {
        private string configString;
        private string cfg2;
        private PluginConfiguration cofiguration;
        private Guid InstanceId = Guid.NewGuid();
        public ValidationPlugin(string unsecureString, string secureString)
        {
            this.configString = unsecureString;
            this.cfg2 = secureString;
            this.cofiguration = PluginConfiguration.Desrialize(this.configString);
            try
            {
                this.cofiguration = new JavaScriptSerializer().Deserialize<PluginConfiguration>(this.configString);
            }
            catch { }
        }
        public ValidationPlugin()
        {

        }
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
                    (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            PluginHelper helper = null;
            try
            {
                helper = new PluginHelper(this, serviceProvider, this.configString, this.InstanceId);
                helper.Log("Plugin Exceution Starts Config:{0}", this.configString);
                tracingService.Trace(string.Format("Hi there: {0}", this.configString));
                // Obtain the execution context from the service provider.
                IPluginExecutionContext context = (IPluginExecutionContext)
                    serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.IsExecutingOffline || context.IsOfflinePlayback)
                    return;
                // The InputParameters collection contains all the data passed 
                // in the message request.
                if (context.InputParameters.Contains("Target") &&
                    context.InputParameters["Target"] is Entity)
                {
                    Entity entity = (Entity)context.InputParameters["Target"];
                    Entity pre_image = context.PreEntityImages.Contains("Target") ? context.PreEntityImages["Target"] as Entity : null;
                    Entity post_image = context.PostEntityImages.Contains("Target") ? context.PostEntityImages["Target"] as Entity : null;

                    helper.Send(entity, pre_image, post_image);
                }
                tracingService?.Trace(helper?.GetLog());
            }
            catch (Exception err)
            {
                tracingService.Trace("An error occured while trying to pump message. Error:{0}", err.Message);
                if (!helper.Handle(err))
                {
                    if (helper?.PluginConfiguration.Trace ?? false)
                        throw new Exception(err.Message + "\r\n" + "Log:" + helper?.GetLog());
                    else
                        throw;
                }

            }
            helper?.FlushLogs();

        }
    }

    /// <summary>
    /// POCO class for plugin configuration that is used
    /// to control the behaviour of the plugin while
    /// activated for a specific processing step.
    /// A serializtion of it will be stored in the
    /// configuration field of a proccessing step.
    /// Upon intstantiation, the plugin recieves a copy 
    /// of this text in its constructor.
    /// </summary>
    public class PluginConfiguration
    {
        /// <summary>
        /// Url of the server that will be used by the
        /// plugin to send messages to the system.
        /// e.g. "http://server:5050"
        /// </summary>
        public string WebApiUrl { get; set; }

        /// <summary>
        /// If true the plugin will create a log entry
        /// as a crm 'Task' activity,
        /// </summary>
        public bool Trace { get; set; }

        /// <summary>
        /// If true the log will be thrown as an 
        /// exception that may be caought for test
        /// puproses.
        /// </summary>
        public bool TraceThrow { get; set; }

        /// <summary>
        /// If set, the plugin will throw exceptions
        /// and roll back changes in case of errors. 
        /// It should be used for critical events that if missed
        /// will damage the system integrity.
        /// </summary>
        public bool IsCritical { get; set; }
        /// <summary>
        /// When set the plugin will work in Synch mode.
        /// In this mode the plugin will wait for the 
        /// result of web api call.
        /// </summary>
        public bool SendSynch { get; set; }

        public int TimeOut { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Serialize()
        {

            return new JavaScriptSerializer().Serialize(this);
        }
        public static PluginConfiguration Desrialize(string text)
        {
            PluginConfiguration result = null;
            try
            {
                result = new JavaScriptSerializer().Deserialize<PluginConfiguration>(text);
            }
            catch { }
            if (result == null)
            {
                result = new PluginConfiguration();
            }
            return result;
        }
    }

    public class KeyValue
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Type { get; set; }
        public KeyValue() { }
        public KeyValue(string key, object value)
        {
            if (value is EntityReference _val)
            {
                value = new JsonSerializableEntity._EntityReference
                {
                    Id = _val.Id,
                    LogicalName = _val.LogicalName,
                    Name = _val.Name
                };
            }
            if (value is EntityCollection _col)
            {
                value = new JsonSerializableEntity._EntityCollection()
                {
                    Entities = _col.Entities==null?Array.Empty<JsonSerializableEntity>(): _col.Entities.Select(x => new JsonSerializableEntity(x)).ToArray(),
                };
            }
            
            this.Key = key;
            this.Value = value;
            this.Type = value?.GetType().AssemblyQualifiedName;
        }
    }
    public class ChangeValue
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }


    public class JsonSerializableEntity
    {

        class _Entity
        {
            public string LogicalName { get; set; }
            public Guid Id { get; set; }
            public _Attribute[] Attributes { get; set; }
        }

        class _Attribute
        {
            public string Key { get; set; }
            public object Value { get; set; }
        }
        public class _EntityCollection
        {
            public JsonSerializableEntity[] Entities { get; set; }
        }
        public class _EntityReference
        {
            public Guid Id { get; set; }
            public string LogicalName { get; set; }
            public string Name { get; set; }
        }
        public string LogicalName { get; set; }
        public Guid Id { get; set; }
        public List<KeyValue> Attributes { get; set; }
        public JsonSerializableEntity()
        {

        }
        public JsonSerializableEntity(Entity entity)
        {
            this.SetEntity(entity);
        }

        public JsonSerializableEntity SetEntity(Entity entity)
        {
            if (entity != null)
            {
                this.LogicalName = entity.LogicalName;
                this.Id = entity.Id;
                this.Attributes = entity.Attributes.Select(x => new KeyValue(x.Key, x.Value)).ToList();
            }
            return this;
        }
        public Entity GetEntity()
        {
            var serializer = new JavaScriptSerializer();
            KeyValuePair<string, object> fix(KeyValuePair<string, object> source)
            {
                object _value = source.Value;
                var isJObject = _value != null && _value.GetType() != typeof(string)
                    && _value.ToString().Trim().StartsWith("{");
                if (isJObject)
                {
                    var strValue = _value.ToString();//.Replace("{}","1");
                    OptionSetValue optionSet = null;
                    if (strValue.Contains("\"Value"))
                    {
                        try
                        {
                            optionSet = serializer.Deserialize<OptionSetValue>(strValue.Replace("\"ExtensionData\": {}", "\"AAA\":1"));
                            _value = optionSet;
                        }
                        catch
                        {
                            try
                            {
                                _value = serializer.Deserialize<Money>(strValue.Replace("\"ExtensionData\": {}", "\"AAA\":1"));
                                //_value = optionSet;
                            }
                            catch { }

                        }
                    }
                    if (strValue.Contains("Entities"))
                    {
                        var refernces = new List<EntityReference>();
                        foreach (var e in serializer.Deserialize<_EntityCollection>(strValue).Entities)
                        {
                            foreach (var attr in e.Attributes)
                            {
                                if (attr.Key == "partyid")
                                {
                                    var values = attr.Value as Dictionary<string, object>;
                                    if (values.TryGetValue("Id", out var s_id) && values.TryGetValue("LogicalName", out var logicalName) &&
                                        s_id != null && logicalName != null &&
                                        Guid.TryParse(s_id.ToString(), out var id))

                                    {
                                        values.TryGetValue("Name", out var name);
                                        var reference = new EntityReference(logicalName.ToString(), id);
                                        reference.Name = name?.ToString();
                                        refernces.Add(reference);
                                    }
                                }
                                //    if (values!=null )
                                //    {
                                //        if (values.TryGetValue("Id", out var s_id) && values.TryGetValue("LogicalName",out var logicalName) && 
                                //            s_id!=null && logicalName!=null &&
                                //            Guid.TryParse(s_id.ToString(), out var id))

                                //        {
                                //            refernces.Add(new EntityReference(logicalName.ToString(), id));
                                //        }
                                //    }
                            }
                        }
                        if (refernces.Count > 0)
                        {

                            EntityCollection collection = new EntityCollection(refernces.Select(x =>
                            {
                                var en2 = new Entity();
                                en2.Attributes.Add("partyid", x);
                                return en2;
                            }
                            ).ToList());
                            _value = collection;
                        }
                    }
                    else if (strValue.Contains("\"LogicalName"))
                    {
                        try
                        {
                            var _reference = serializer.Deserialize<_EntityReference>(strValue);

                            _value = new EntityReference(_reference.LogicalName, _reference.Id);
                        }
                        catch { }
                    }
                }
                if (_value != null && _value.GetType() == typeof(string) && Guid.TryParse(_value.ToString(), out var guid))
                {
                    _value = guid;
                }

                return new KeyValuePair<string, object>(source.Key, _value);
            }

            KeyValuePair<string, object> fix2(KeyValuePair<string, object> source)
            {
                object _value = source.Value;
                var isJObject = _value != null && _value.GetType() != typeof(string)
                    && _value.ToString().Trim().StartsWith("{");
                if (isJObject)
                {
                    var strValue = _value.ToString();
                    OptionSetValue optionSet = null;
                    if (strValue.Contains("\"Value"))
                    {
                        try
                        {
                            optionSet = serializer.Deserialize<OptionSetValue>(strValue);
                            _value = optionSet;
                        }
                        catch
                        {
                            try
                            {

                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    if (strValue.Contains("\"LogicalName"))
                    {
                        try
                        {
                            var _reference = serializer.Deserialize<_EntityReference>(strValue);

                            _value = new EntityReference(_reference.LogicalName, _reference.Id);
                        }
                        catch { }
                    }
                }

                return new KeyValuePair<string, object>(source.Key, _value);
            }
            var result = new Entity(this.LogicalName, this.Id);
            result.Attributes
                .AddRange(this.Attributes.Select(x => fix(new KeyValuePair<string, object>(x.Key, x.Value))));
            return result;
        }

    }
    public class EntityWebCommandRequestModel
    {
        public string MessageName { get; set; }
        public JsonSerializableEntity Entity { get; set; }
        public JsonSerializableEntity PreImage { get; set; }
        public JsonSerializableEntity PostImage { get; set; }
        public int Mode { get; set; }
        public Guid RequestId { get; set; }
        public int Stage { get; set; }
        public bool IsSynchronous { get; set; }
        public string PluginName { get; set; }
        public Guid ProcessingStepId { get; set; }
        public string PrimaryEtityLogicalName { get; set; }
        public Guid PrimaryEntityId { get; set; }

        public Guid InitiatingUserId { get; set; }
        public string OrganizationName { get; set; }
        public Guid BuisnessUnitIdOfCallingUser { get; set; }


    }
    public class EntityWebCommandResponseModel
    {
        public string Error { get; set; }
        public JsonSerializableEntity Enity { get; set; }
        public Entity Enitty2 { get; set; }

        public string Data { get; set; }
        public List<ChangeValue> Changes { get; set; }
        public Dictionary<string, object> ChangesEx { get; set; }

        ////public List<KeyValuePair>
    }
}

