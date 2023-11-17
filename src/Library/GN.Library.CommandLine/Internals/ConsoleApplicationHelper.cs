using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using GN.Library.Messaging;
using Microsoft.Extensions.Configuration;
using GN.Library;

namespace GN.Library.CommandLines.Internals
{
	public class ConsoleApplicationHelper
	{
		static string ConfigFileName = "./GN.Console/config.json";
		static Config config = null;
		static bool IsConnected = false;
		
		
		public static async Task Main(string[] args)
		{
			var cfg = GetConfig();
			if (args.Length > 0)
				cfg.Url = args[0];
			Context ctx = null;
			var host = CreateHost();
			var count = 0;
			while (true)
			{
				if (!EnsureConnection(cfg, false))
				{
					cfg.Url = "";
					continue;
				}
				ctx = ctx ?? new Context();
				System.Console.Write("$ (Enter for exit): ");
				var command = System.Console.ReadLine();
				if (command != null && command.Trim().ToLowerInvariant() == "help")
				{
					command = "--help";
				}
				if (command == "exit")
					break;
				var result = SendCommand2(cfg.Url, command, ctx).ConfigureAwait(false).GetAwaiter().GetResult();
				System.Console.WriteLine(result?.Replace("\\t","\t"));
			}

			await host.StopAsync();
		}
		static string GetConfigFileName()
		{
			var result = Path.GetFullPath(
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				ConfigFileName));
			if (!Directory.Exists(Path.GetDirectoryName(result)))
				Directory.CreateDirectory(Path.GetDirectoryName(result));
			return result;
		}
		static Config GetConfig(bool refresh = false)
		{
			if (config == null || refresh)
			{
				var configFile = GetConfigFileName();
				config = new Config();
				if (File.Exists(configFile))
				{
					try
					{
						config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
					}
					catch { }
				}
			}
			return config;
		}
		static bool TryConnect(string url, out string serverInfo, int trialCount = 3)
		{

			serverInfo = null;
			System.Console.WriteLine("Trying to connect to server at:{0}", url);
			for (int i = 0; i < trialCount; i++)
			{
				System.Console.WriteLine($"Trial: {i}");
				try
				{
					var result1 = SendCommand(url, "ping", "", new Context()).ConfigureAwait(false).GetAwaiter().GetResult();
					if (result1 != null)
					{
						serverInfo = result1.Reply;
						break;
					}
				}
				catch
				{
				}
			}
			return !string.IsNullOrWhiteSpace(serverInfo);
		}
		static bool EnsureConnection(Config cfg, bool refersh)
		{
			if (!IsConnected || refersh)
			{
				//var cfg = GetConfig();
				while (!IsConnected)
				{
					while (string.IsNullOrEmpty(cfg.Url) || !Uri.IsWellFormedUriString(cfg.Url, UriKind.Absolute))
					{
						System.Console.Write("Enter Server Url :");
						cfg.Url = System.Console.ReadLine();
						if (string.IsNullOrEmpty(cfg.Url))
						{
							break;
						}

					}
					if (!string.IsNullOrWhiteSpace(cfg.Url) && Uri.IsWellFormedUriString(cfg.Url, UriKind.Absolute))
					{
						IsConnected = TryConnect(cfg.Url, out var serverInfo);
						if (IsConnected)
						{
							System.Console.WriteLine(
								$"Successfully Connected to Server. Url:{cfg.Url}, Info:{serverInfo}");
							File.WriteAllText(GetConfigFileName(), Newtonsoft.Json.JsonConvert.SerializeObject(cfg));
						}
						else
						{
							cfg.Url = "";
						}
					}
				}

			}
			return IsConnected;

		}
		internal static async Task<string> SendCommand2(string url, string args, Context context)
		{
			var result = string.Empty;
			HttpClient client = new HttpClient(new HttpClientHandler()
			{
				UseDefaultCredentials = true
			})
			{
				BaseAddress = new Uri(url),
			};
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			try
			{
				//var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(_req), Encoding.UTF8, "application/json");
				var resp = await client.GetAsync($"api/ExecuteCommand?command={args}").ConfigureAwait(false);
				if (resp.IsSuccessStatusCode && resp.Content != null)
				{
					result = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
					result = result.Replace("\\r", "\r")
						.Replace("\\n", "\n");

				}
				else
					throw new Exception("Unable to call WebAPI");
			}
			catch (Exception err)
			{
				System.Console.WriteLine(
					$"Connection Error: {err.Message} ");
				IsConnected = false;
				//GetConfig().Url = "";
			}
			return result;
		}
		internal static async Task<CommandLineResponse> SendCommand(string url, string commandName, string args, Context context)
		{
			CommandLineResponse result = null;
			HttpClient client = new HttpClient(new HttpClientHandler()
			{
				UseDefaultCredentials = true
			})
			{
				BaseAddress = new Uri(url),
			};
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));
			try
			{
				var _req = new WebRequest();
				_req.Request = commandName;
				_req.Data = Newtonsoft.Json.JsonConvert.SerializeObject(new CommandLineWebCommand()
				{
					Args = args,
					Context = context
				});
				var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(_req), Encoding.UTF8, "application/json");
				var resp = await client.PostAsync("api/webcommand", content).ConfigureAwait(false);
				if (resp.IsSuccessStatusCode && resp.Content != null)
				{
					var strResponse = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
					var _resp = Newtonsoft.Json.JsonConvert.DeserializeObject<WebResponse>(strResponse);
					result = Newtonsoft.Json.JsonConvert.DeserializeObject<CommandLineResponse>(_resp.Data);
				}
				else
					throw new Exception("Unable to call WebAPI");
			}
			catch (Exception err)
			{
				System.Console.WriteLine(
					$"Connection Error: {err.Message} ");
				IsConnected = false;
				//GetConfig().Url = "";
			}
			return result;
		}

		public static Microsoft.Extensions.Hosting.IHost CreateHost()
		{
			var host = AppHost.GetHostBuilder()
				.ConfigureAppConfiguration(c =>
				{
					c.AddJsonFile("appsettings.json");
				})
				.ConfigureServices((ctx, s) =>
				{
					s.AddGNLib(ctx.Configuration, cfg => { });
					//				s.AddGnMessageBus(ctx.Configuration, cfg =>
					//				{
					//					cfg.ConfigureRabbitMQ(cfg => { })
					//					.AddRabbitMQBus(settings: s =>
					//					{
					//						s.PurgeOnStartup = true;
					//					},
					//					configure: cfg =>
					//					{


					//						cfg.AddConsumer<LogHandler>(c =>
					//						{

					//						}).Endpoint(e =>
					//						{
					//							e.Temporary = true;
					//							e.InstanceId = $"{Environment.MachineName}-console";
					//						});
					//					}, configureEx: cfg =>
					//					{
					//						//cfg.AutoDelete = true;
					//						//cfg.Durable = false;
					//					}, configureExEx: (a, b) =>
					//					{

					//						b.ReceiveEndpoint("console_queue", x =>
					//						{
					//							x.Consumer<LogHandler>();
					//							x.Durable = false;
					////							x.Bind<LogHandler>();
					//							x.PurgeOnStartup = true;
					//							x.AutoDelete = true;
					//						});

					//					});
					//				});
				})
				.Build()
				.UseGnLib();
			return host;

		}
	}






	class Config
	{
		public string Url { get; set; }
	}
	class WebRequest
	{
		public string Request { get; set; }
		public string Data { get; set; }
	}
	class WebResponse
	{
		public string Data { get; set; }

	}
	class Context
	{
		public Dictionary<string, object> Headers { get; set; }
		public Context()
		{
			this.Headers = new Dictionary<string, object>();
		}
	}
	class CommandLineWebCommand
	{
		public string Args { get; set; }
		public Context Context { get; set; }
	}
	class CommandLineResponse
	{
		public string Log { get; set; }
		public string Reply { get; set; }
		public Context Context { get; set; }
	}

}
