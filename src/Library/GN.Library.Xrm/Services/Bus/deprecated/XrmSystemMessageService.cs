using GN.Library.Contracts_Deprecated;
using GN.Library.Messaging;
using GN.Library.Xrm.GnLibSolution;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Xrm.Sdk;
using GN.Library.TaskScheduling;
using Microsoft.Extensions.DependencyInjection;
using GN.Library.Messaging.deprecated;

namespace GN.Library.Xrm.Services.Bus
{
	class XrmSystemMessageService : HostedService
	{
		private readonly ILogger logger;
		private readonly IMessageBus_Depreacted bus;
		private readonly IXrmDataServices dataServices;
		private readonly IServiceProvider serviceProvider;
		private const string SYSTEM_MESSAGE_LOGICAL_NAME = XrmSystemMessage.Schema.LogicalName;

		public XrmSystemMessageService(ILogger<XrmSystemMessageService> logger, 
			IXrmDataServices dataServices, IServiceProvider serviceProvider)
		{
			this.logger = logger;
			this.dataServices = dataServices;
			this.serviceProvider = serviceProvider;
			this.bus = this.serviceProvider.GetServiceEx<IMessageBus_Depreacted>();
		}
		private async Task HandleReply(IMessageContext_Deprecated<XrmSystemMessageReplyModel> message)
		{
			await Task.CompletedTask;

			var task = Task.Delay(5 * 1000).ContinueWith(t =>
			{
				try
				{
					var reply = message.GetMessage();
					if (reply != null && reply.GetRequestId().HasValue)
					{
						using (var ctx = AppHost.Context.Push())
						{
							var repo = ctx.AppServices.GetService<IXrmRepository<XrmSystemMessage>>();
							var request = repo.Queryable.FirstOrDefault(x => x.SystemMessageId == reply.GetRequestId().Value);
							if (request != null)
							{
								if (request.State == DefaultStateCodes.Active)
								{
									FinalizeMessage(request, reply, repo);
								}
							}
						}
					}
				}
				catch (Exception err)
				{
					this.logger.LogError(
						"An error occured while trying to Handle SystemMessageReplyModel: {0}, Err:{1}", message, err);
				}


			});
			return;
		}


		private void FinalizeMessage(XrmSystemMessage message, XrmSystemMessageReplyModel reply, IXrmRepository<XrmSystemMessage> repo)
		{
			if (message != null && message.State == DefaultStateCodes.Active)
			{
				//using (var scope = AppHost.Context.Push())
				{
					//var repo = scope.AppServices.GetService<IXrmRepository<XrmSystemMessage>>();
					if (reply != null)
					{
						message.DateResult = reply?.DateResult;
						message.StrResult = reply?.StrResult;
						message.IntResult = reply?.IntResult;
						message.DateTimeResult = reply?.DateTimeResult;
						message.DecimalResult = reply?.DecimalResult;
						message.FloatResult = reply?.FloatResult;
						message.Error = reply?.Error;
						repo.Update(message);
					}
					if (message.Status == XrmSystemMessage.Schema.StatusCodes.Active_Processed_With_Error || reply != null && reply.Failed)
					{
						message.SetFailed(repo);
					}
					else if (message.Status == XrmSystemMessage.Schema.StatusCodes.Active_Processed)
					{
						message.SetCompleted(repo);
					}
				}
			}
		}

		private async Task HandleXrmMessage(XrmMessage message, bool IsSynchCall = true)
		{
			await Task.CompletedTask;
			IWaitContext<XrmSystemMessageReplyModel> waitContext = null;
			Task<IMessageContext_Deprecated<XrmSystemMessageReplyModel>> task = Task.FromResult<IMessageContext_Deprecated<XrmSystemMessageReplyModel>>(null);
			Guid requestId = Guid.Empty;
			IsSynchCall = message.IsSync;
			XrmSystemMessageReplyModel reply = null;
			bool isCompleted = false;
			try
			{
				if (message.Entity != null)
				{
					var request_message = message.Entity.ToXrmEntity<XrmSystemMessage>();
					var isDraft = request_message.Status == XrmSystemMessage.Schema.StatusCodes.Active_Draft;
					if (isDraft)
						return;
					var isReady = request_message.Status == XrmSystemMessage.Schema.StatusCodes.Active_Ready;
					if (isReady)
					{
						requestId = message.PrimraryEntityId;
						int timeout = request_message.Timeout.HasValue
							? request_message.Timeout.Value
							: request_message.IsSyncCommand()
								? 45 * 1000
								: 60 * 60 * 1000;
						if (request_message.IsCommand())
						{
							waitContext = this.bus.CreateWaiter<XrmSystemMessageReplyModel>(x =>
							{
								var _m = x.GetMessage();
								Guid? replyToRequestId = _m.Request == null ? _m.SystemEventId : _m.Request.SystemEventId;
								return Task.FromResult(replyToRequestId.HasValue && replyToRequestId.Value == requestId);
							}, timeout);
							task = waitContext.GetTask();// this._cts.Token);
						}
						else
						{
							task = Task.FromResult<IMessageContext_Deprecated<XrmSystemMessageReplyModel>>(null);

						}
						await this.bus.CreateMessage(request_message.ToXrmMessageModel())
						 .Configure(reset: false, topic: request_message.Topic)
						 .Publish();

						this.logger.LogDebug(
							"SystemMessage Published: {0}", request_message);
					}
					else
					{
						/// No need to publish...
					}

					var task2 = task.ContinueWith(r =>
					{
						var _isSyncCall = IsSynchCall;

						Exception error = null;
						if (r.IsFaulted)
						{
							error = r.Exception;
						}
						else if (r.IsCanceled)
						{
							error = new OperationCanceledException();
						}
						else if (r.IsCompleted)
						{
							reply = r.Result?.GetMessage();
							isCompleted = true;
							if (reply == null)
							{
								//error = new Exception(
								//	"Unexpected Result");
							}
							else if (reply.Failed)
							{
								error = new Exception(reply.Error);
							}
						}
						if (IsSynchCall && request_message.IsSyncCommand())
						{
							if (error == null)
							{
								if (reply != null)
								{
									message.Change(XrmSystemMessage.Schema.StrResult, reply.StrResult);
									message.Change(XrmSystemMessage.Schema.IntResult, reply.IntResult);
									message.Change(XrmSystemMessage.Schema.DateResult, reply.DateResult);
									message.Change(XrmSystemMessage.Schema.FloatResult, reply.FloatResult);
									message.Change(XrmSystemMessage.Schema.DateTimeResult, reply.DateTimeResult);
									message.Change(XrmSystemMessage.Schema.DecimalResult, reply.DecimalResult);
								}
								message.Change("statuscode", (int)XrmSystemMessage.Schema.StatusCodes.Active_Processed);
							}
							else
							{
								message.Change(XrmSystemMessage.Schema.Error, error.GetBaseException().Message);
								message.Change("statuscode", (int)XrmSystemMessage.Schema.StatusCodes.Active_Processed_With_Error);
							}
						}
						else if (IsSynchCall && request_message.IsAsyncCommand())
						{
							using (var ctx = AppHost.Context.Push())
							{
								if (error == null)
								{
									request_message.Status = XrmSystemMessage.Schema.StatusCodes.Active_Processed;
								}
								else
								{
									request_message.Status = XrmSystemMessage.Schema.StatusCodes.Active_Processed_With_Error;
								}
								FinalizeMessage(request_message, reply, ctx.AppServices.GetService<IXrmRepository<XrmSystemMessage>>());
							}
						}
						else if (IsSynchCall && request_message.IsEvent())
						{
							

						}
						if (!IsSynchCall && request_message.IsSyncCommand())
						{
							using (var ctx = AppHost.Context.Push())
							{
								FinalizeMessage(request_message, null, ctx.AppServices.GetService<IXrmRepository<XrmSystemMessage>>());
							}
						}
						if (!IsSynchCall && request_message.IsEvent())
						{
							request_message.Status = XrmSystemMessage.Schema.StatusCodes.Active_Processed;
							using (var ctx = AppHost.Context.Push())
							{
								FinalizeMessage(request_message, null, ctx.AppServices.GetService<IXrmRepository<XrmSystemMessage>>());
							}
						}


					});
					if (request_message.IsSyncCommand())
					{
						await task2;
					}
					else if (IsSynchCall)
					{

						message.Change("statuscode",
							isCompleted
							? (int)XrmSystemMessage.Schema.StatusCodes.Active_Processed
							: (int)XrmSystemMessage.Schema.StatusCodes.Active_InProgress);
					}

				}
			}
			catch (Exception err)
			{
				this.logger.LogError(
					"An error occured while trying to handle SystemMessage update. Message:{0}, Err:{1}", message, err.GetBaseException().Message);

			}

		}

		private async Task HandleSystemFunctions(IMessageContext_deprecated message)
		{
			await Task.CompletedTask;
			if (message != null && message.GetMessageType() == typeof(XrmSystemMessageModel))
			{
				var model = message.GetMessageEx<XrmSystemMessageModel>();
				XrmSystemMessageReplyModel result = null;
				switch (message.Topic)
				{
					case "cmd.add":
						result = SystemFunctions.Add(model);
						break;
					case "cmd.sub":
						//await Task.Delay(60 * 1000);
						result = model.CreateReply();
						result.Failed = true;
						result.Error = "Div By Zero";
						break;
				}
				if (result != null)
				{
					var ttt = Task.Delay(20 * 1000).ContinueWith(r =>
					  {
						  this.bus.CreateMessage(result)
							  .Publish();
					  });
				}
			}
		}

		private async Task DoStartAync(XrmMessageBus xrmBus)
		{
			await Task.CompletedTask;
			if (this.dataServices.GetSchemaService().EntityExists(XrmSystemMessage.Schema.LogicalName))
			{
				await this.bus.SubscribeAsync(cfg =>
				{
					cfg.Topic = "cmd.*";
					cfg.Handler = ctx => this.HandleSystemFunctions(ctx);
				});
				await this.bus.SubscribeAsync<XrmSystemMessageReplyModel>(cfg =>
				{
					cfg.Handler = ctx => this.HandleReply(ctx);
				});
				xrmBus.Subscribe(cfg =>
				{
					cfg.AddFilter(f =>
					{
						f.Message = Plugins.PluginMessageTypes.Create;
						f.TargetEntityName = XrmSystemMessage.Schema.LogicalName;
						f.Stage = Plugins.PluginMessageStages.PreValidation;
						//f.PluginConfiguration.Trace = true;
						//f.PluginConfiguration.TraceThrow = true;
						f.PluginConfiguration.SendSynch = true;
					});
					cfg.Handler = async ctx =>
					{
						//ctx.Changes.Add("gnlic_str1", "newvalue");
						//ctx.Change("gnlib_str1", "newvalue");
						await this.HandleXrmMessage(ctx);

					};
				});
				xrmBus.Subscribe(cfg =>
				{
					cfg.AddFilter(f =>
					{
						f.Message = Plugins.PluginMessageTypes.Create;
						f.TargetEntityName = XrmSystemMessage.Schema.LogicalName;
						f.Stage = Plugins.PluginMessageStages.PostOperation;
					});
					cfg.Handler = ctx => this.HandleXrmMessage(ctx);
				});


			}
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			var xrmBus = this.serviceProvider.GetServiceEx<XrmMessageBus>();
			if (this.dataServices.GetSchemaService().EntityExists(XrmSystemMessage.Schema.LogicalName))
			{
				if (this.bus != null)
				{
					await this.bus.SubscribeAsync(cfg =>
					{
						cfg.Topic = "cmd.*";
						cfg.Handler = ctx => this.HandleSystemFunctions(ctx);
					});
					await this.bus.SubscribeAsync<XrmSystemMessageReplyModel>(cfg =>
					{
						cfg.Handler = ctx => this.HandleReply(ctx);
					});
				}
				xrmBus.Subscribe(cfg =>
				{
					cfg.AddFilter(f =>
					{
						f.Message = Plugins.PluginMessageTypes.Create;
						f.TargetEntityName = XrmSystemMessage.Schema.LogicalName;
						f.Stage = Plugins.PluginMessageStages.PreValidation;
						//f.PluginConfiguration.Trace = true;
						//f.PluginConfiguration.TraceThrow = true;
						f.PluginConfiguration.SendSynch = true;
					});
					cfg.Handler = async ctx =>
					{
						//ctx.Changes.Add("gnlic_str1", "newvalue");
						//ctx.Change("gnlib_str1", "newvalue");
						await this.HandleXrmMessage(ctx);

					};
				});
				xrmBus.Subscribe(cfg =>
				{
					cfg.AddFilter(f =>
					{
						f.Message = Plugins.PluginMessageTypes.Create;
						f.TargetEntityName = XrmSystemMessage.Schema.LogicalName;
						f.Stage = Plugins.PluginMessageStages.PostOperation;
					});
					cfg.Handler = ctx => this.HandleXrmMessage(ctx);
				});


			}

			await base.StartAsync(cancellationToken);
		}
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			return base.StopAsync(cancellationToken);
		}
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			await Task.Delay(1000);
		}
	}
}
