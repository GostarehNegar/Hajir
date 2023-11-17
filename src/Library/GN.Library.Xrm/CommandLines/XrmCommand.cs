using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GN.Library.CommandLines_deprecated;
using Microsoft.Extensions.CommandLineUtils;

namespace GN.Library.Xrm.CommandLines
{
    class XrmConfigCommand : CommandLine
    {
        protected override string Name => "config";
        private CommandOption connectionStringOption;
        public override void DoConfigure(CommandLineApplicationEx command)
        {
            this.connectionStringOption = command
                .Option("-cs|--connectionstring", "Sets Xrm Connection String. Example: 'Url=http://server/organization, User=user, Password=pass, domain=mydomain'", CommandOptionType.SingleValue);
        }

        public async override Task<int> DoExecute(CommandLineApplicationEx command)
        {
            var result = await Task.FromResult(0).ConfigureAwait(false);
            if (this.connectionStringOption.HasValue())
            {
                var connectionString = this.connectionStringOption.Value();
                throw new NotImplementedException();
               
                //XrmSettings.Current.ConnectionString = c.ConnectionString;
                //var service = AppHost.GetService<IXrmOrganizationService>();
                //var _result = service.TestConnection();
                ////AppHost.Context.Configuration.Save();
                //if (_result)
                //{
                //    command.WriteLine(
                //        $"Connection string successfully changed and tested. " +
                //        $"ConnectionString: '{c.ConnectionString}'");
                //}
                //else
                //{
                //    command.WriteLine(
                //        $"Connection string successfully changed but the test for the new connection failed." +
                //        $"ConnectionString: ''");
                //    throw new Exception(
                //        $"Invaid Xrm ConnectionString: {connectionString}");

                //}


            }
            return result;

        }
    }
    class XrmStatusCommand :CommandLine
    {
        protected override string Name => "status";

        public override void DoConfigure(CommandLineApplicationEx command)
        {
            command.Description = "Reports Current Stautus of Xrm Module.";
        }

        public async override Task<int> DoExecute(CommandLineApplicationEx command)
        {
            var result = await Task.FromResult(0).ConfigureAwait(false);
            var service = AppHost.GetService<IXrmOrganizationService>();
            command.WriteLine("Xrm Status:");
            command.WriteLine("Connection String:\t '{0}' ", service.ConnectionString.ConnectionString);
            command.WriteLine("Connection Test:\t '{0}'", service.TestConnection(true));
            command.WriteLine("Service User Guid:\t '{0}'", service.GetServiceUserGuid());
            return result;

        }
    }
    class XrmCommand : CommandLine
    {
        protected override string Name => "Xrm";

        public override void DoConfigure(CommandLineApplicationEx command)
        {
            command.Description = "Configuration and status of Xrm module.";
            var _command = new XrmConfigCommand();
            _command.Configure(command);
            new XrmStatusCommand()
               .Configure(command);

        }

        public async override Task<int> DoExecute(CommandLineApplicationEx command)
        {
            var result = await Task.FromResult(0).ConfigureAwait(false);
            var _command = new XrmStatusCommand();
            await _command.Execute(command).ConfigureAwait(false);
            return result;

        }
    }
}
