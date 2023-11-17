using GN.Library.Messaging;
using GN.Library.ServerManagement;
using GN.Library.ServiceDiscovery;
using GN.Library.Shared.ServiceDiscovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.BlazorServer.ServiceManagment
{
    public interface IServiceManagementBlazorServices
    {
        IServiceProvider ServiceProvider { get; }
        IObservable<T> GetObservable<T>();
        T GetService<T>();
        NodeData GetNodeData(int processId);
        ProcessWrapper GetProcess(int processId);
    }
    class ServiceManagementBlazorServices : IServiceManagementBlazorServices
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public T GetService<T>() => this.ServiceProvider.GetServiceEx<T>();
        public ServiceManagementBlazorServices(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
        }
        public IMessageBus Bus => GetService<IMessageBus>();


        public IObservable<NodeStatusData> CreateNodeStatusObservable()
        {
            var result = System.Reactive.Linq.Observable.Create<NodeStatusData>(async o =>
            {
                return await Bus.CreateSubscription()
                   .UseTopic(LibraryConstants.Subjects.ServiceDiscovery.HeartBeatEvent)
                   .UseHandler(ctx =>
                   {
                       var NodeStatus = this.GetService<IServiceDiscovery>().NodeStatus;
                       o.OnNext(NodeStatus);
                       return Task.CompletedTask;
                   })
                   .Subscribe();
            });
            return result;


        }

        public IObservable<T> GetObservable<T>()
        {
            if (typeof(T) == typeof(NodeStatusData))
            {
                return CreateNodeStatusObservable() as IObservable<T>;
            }
            throw new Exception("Unavailable");
        }
        public NodeData GetNodeData(int processId)
        {
            var nodeStatus = this.ServiceProvider
                .GetServiceEx<IServiceDiscovery>()
                .NodeStatus;
            if (nodeStatus.Node.ProcessId == processId.ToString())
                return nodeStatus.Node;
            return nodeStatus.Peers.Values
                .FirstOrDefault(x => x.ProcessId == processId.ToString());
        }
        public ProcessWrapper GetProcess(int processId)
        {

            return this.ServiceProvider.GetServiceEx<ServerProcessControler>()
                .GetProcessById(processId);
        }
    }

}
