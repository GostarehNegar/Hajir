using GN.Library.Win32.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GN.Library.Win32.Hosting
{
    public class WindowsServiceBuilder
    {
        private IWebHostBuilder webHostBuilder;
        private IHostBuilder hostBuilder;
        public string ServiceName { get; set; }
        public string ServiceDisplayName { get; set; }
        public string ServiceArguments { get; set; }
        internal string[] Args;

        internal WindowsServiceBuilder(string[] args)
        {
            this.Args = args;
        }

        public WindowsServiceBuilder UseWebHostBuilder(IHostBuilder builder)
        {
            this.hostBuilder = builder;
            return this;
        }
        public WindowsServiceBuilder UseWebHostBuilder(IWebHostBuilder builder)
        {
            this.webHostBuilder = builder;
            return this;
        }
        public WindowsServiceBuilder ConfigureWindowsService(string name, string displayName, string args = null)
        {

            this.ServiceName = name;
            this.ServiceDisplayName = displayName;
            this.ServiceArguments = args;
            return this;

        }
        public IWindowsServiceHost Build(Func<IWebHost, IWebHost> func = null)
        {

            return new WindowsServiceHost(this, func == null ? webHostBuilder?.Build().UseGNLib() : func(webHostBuilder?.Build().UseGNLib()), hostBuilder?.Build().UseGNLib());
        }
        public IWindowsServiceHost Build1(Func<IHost, IHost> func = null)
        {

            return new WindowsServiceHost(this, null, func == null ? hostBuilder?.Build() : func(hostBuilder?.Build()));
        }
    }
}

