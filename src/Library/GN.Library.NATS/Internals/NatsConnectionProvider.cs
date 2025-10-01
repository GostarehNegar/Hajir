using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GN.Library.Nats;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
namespace GN.Library.Nats.Internals
{
    internal class NatsConnectionProvider : BackgroundService, INatsConnectionProvider
    {
        private readonly IServiceProvider serviceProvider;
        private readonly NatsOptions options;
        private readonly ILogger<NatsConnectionProvider> logger;
        private Process natsProcess;
        public NatsConnectionProvider(
            IServiceProvider serviceProvider,
            NatsOptions options,
            ILogger<NatsConnectionProvider> logge)
        {
            this.serviceProvider = serviceProvider;
            this.options = options;
            this.logger = logge;
        }
        public GN.Library.Nats.NatsExtensions.INatsConnectionEx CreateConnection(string name = null, bool Throw = true)
        {


            try
            {
                var con = this.serviceProvider.CreateNatsConnection(null, false, cfg =>
                    cfg with
                    {
                        Url = this.options.Url?? cfg.Url,
                        ConnectTimeout = TimeSpan.FromSeconds(10)
                    }
                );
                var state = con.Core.ConnectionState;
                return con;
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    $"An error occured while trying to connect to nats. Url:{this.options.Url}. Err:{ex.Message}");
                if (Throw)
                    throw;
            }
            return null;
        }

        public async Task<bool> IsAvailable()
        {
            var con = this.CreateConnection(Throw: false);


            return con != null && con.Core.ConnectionState == global::NATS.Client.Core.NatsConnectionState.Open;

        }
        public async Task<bool> AutoStart()
        {
            try
            {
                var zipFileName = "nats.zip";
                var nats_server = "nats-server.exe";
                var nats_config = "nats.config.yaml";
                var nats_server_fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), nats_server));
                var nats_config_fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), nats_config));
                var nats_zip_fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), zipFileName));
                this.logger.LogInformation(
                    $"Trying to start nats-server. Path:{nats_server}");
                if (!File.Exists(nats_server_fullPath))
                {
                    if (!File.Exists(nats_zip_fullPath))
                    {
                        throw new FileNotFoundException(zipFileName);
                    }
                    this.logger.LogInformation(
                        $"Extracting 'nats-server' From:{nats_zip_fullPath}. To:{nats_server_fullPath}");
                    var archive = ZipFile.Open(nats_zip_fullPath, ZipArchiveMode.Read)
                            .Entries.FirstOrDefault(x => x.Name == nats_server);
                    archive.ExtractToFile(nats_server_fullPath, true);
                }
                if (!File.Exists(nats_server_fullPath))
                {
                    throw new FileNotFoundException(nats_server_fullPath);
                }
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = nats_server_fullPath;
                startInfo.UseShellExecute = false;
                if (File.Exists(nats_config_fullPath))
                {
                    startInfo.Arguments = $"-c {nats_config_fullPath}";
                }
                else
                {
                    startInfo.Arguments = $"-js";
                }
                this.natsProcess = new Process();
                this.natsProcess.StartInfo = startInfo;
                this.natsProcess.ErrorDataReceived += (a, b) =>
                {

                };
                this.natsProcess.Start();
                await Task.Delay(1000);

            }
            catch (Exception err)
            {
                this.logger.LogError(
                    $"An error occured while trying to auto-start nats. Err:{err.Message}");
            }
            return false;
        }

        private Task Monitor(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {

                this.logger.LogInformation(
                    $"Nats connection provider started. Options:{this.options}");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var started = await this.IsAvailable() || (await AutoStart() && await IsAvailable());
                        var con = this.CreateConnection();
                        if (!started)
                        {
                            this.logger.LogInformation(
                                $"Nats connection provider successfully started. Server Url:{con.Core.Opts.Url}");
                        }
                        await Task.Delay(1 * 60 * 1000);
                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(
                            $"An error occured while trying to start nats connection provider. Err:{ex.Message} ");
                    }
                }



            });
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.WhenAll(this.Monitor(stoppingToken));

        }

        public async Task<NatsExtensions.INatsConnectionEx> CreateConnectionAsync(string name = null, bool Throw = true)
        {
            var ready = await this.IsAvailable() || (await this.AutoStart() && await this.IsAvailable());
            return this.CreateConnection(name, Throw);
        }
    }
}
