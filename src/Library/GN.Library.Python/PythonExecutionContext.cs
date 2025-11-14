using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GN.Library.Python
{

    public class PythonExecutionContext : IDisposable
    {
        public class ExecutionResult
        {
            public int ExitCode { get; set; }
            public string Error { get; set; }
            //public string Output { get; set; }
        }
        public ILogger Logger { get; private set; }
        public async Task<string> ReadOutput()
        {
            if (this.Process != null && this.Process.StartInfo.RedirectStandardOutput)
            {
                return await this.Process.StandardOutput.ReadToEndAsync();
            }
            return "";
        }



        private bool _redirectOutput = false;
        private bool _skipRequirements = false;
        public Process Process { get; private set; }
        public string WorkingPath { get; private set; }
        public string VirtualEnvironmentFolder { get; private set; } = ".venv";
        public string VirtualEnvironmentPath => Path.Combine(this.WorkingPath, this.VirtualEnvironmentFolder);
        private IServiceScope serviceScope;
        public IServiceProvider ServiceProvider => this.serviceScope.ServiceProvider;
        public PythonExecutionContext(IServiceProvider serviceProvider)
        {
            this.serviceScope = serviceProvider.CreateScope();
            this.Logger = this.ServiceProvider.GetService<ILoggerFactory>().CreateLogger(this.GetType());

        }
        public PythonExecutionContext WithWoringPath(string path)
        {
            this.WorkingPath = Path.GetFullPath(path);
            return this;
        }
        public PythonExecutionContext WithWoringPath(bool value)
        {
            this._redirectOutput = value;
            return this;
        }

        public PythonExecutionContext WithoutRequirements()
        {
            this._skipRequirements = true;
            return this;
        }

        public static PythonExecutionContext Create(IServiceProvider serviceProvider)
        {
            return new PythonExecutionContext(serviceProvider);
        }
        public string PythonPath =>
            File.Exists(Path.Combine(this.VirtualEnvironmentPath, "scripts\\python.exe"))
                ? Path.Combine(this.VirtualEnvironmentPath, "scripts\\python.exe")
                : "python.exe";
        public async Task<ExecutionResult> ExecutePythonCode(string code)
        {
            var result = new TaskCompletionSource<ExecutionResult>();
            var process = new Process();
            this.Process?.Dispose();
            this.Process = process;
            if (!Directory.Exists(this.WorkingPath))
            {
                throw new FileNotFoundException($"Working Directory Not Found:{this.WorkingPath} ");
            }
            try
            {
                process.StartInfo = new ProcessStartInfo
                {
                    Arguments = code,
                    FileName = PythonPath,
                    UseShellExecute = false,
                    WorkingDirectory = this.WorkingPath,
                    //RedirectStandardError = true,
                    //RedirectStandardOutput = this._redirectOutput,
                };
                process.StartInfo.Environment["PATH"] = this.VirtualEnvironmentPath + "\\Scripts" + ";" + process.StartInfo.Environment["PATH"];
                process.StartInfo.Environment["VIRTUAL_ENV"] = this.VirtualEnvironmentPath;
                if (!process.Start())
                {
                    throw new Exception($"Failed To Start. StartIngo:{process.StartInfo}");
                }
                process.WaitForExit();
                //while (!process.HasExited)
                //{
                //    await Task.Delay(10);
                //}
                if (process.ExitCode != 0)
                {
                    var _err = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"{process.ExitCode} {_err}");
                }
                result.SetResult(new ExecutionResult
                {
                    //Error = await process.StandardError.ReadToEndAsync(),
                    //Output = await process.StandardOutput.ReadToEndAsync(),
                    ExitCode = process.ExitCode,
                });
                return await result.Task;
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return new ExecutionResult
                {
                    Error = ex.GetBaseException().Message,
                    ExitCode = -1,
                };
            }
        }
        public async Task<PythonExecutionContext> EnsureRequirements()
        {
            await this.EnsureVirtualEnvironment();
            await ExecutePythonCode("-m pip install -r requirements.txt");
            return this;
        }
        public async Task<PythonExecutionContext> EnsureVirtualEnvironment()
        {
            var venvPath = this.VirtualEnvironmentPath;
            if (!Directory.Exists(venvPath))
            {
                await ExecutePythonCode("-m venv .venv");
                await ExecutePythonCode("-m pip install --upgrade pip");
            }
            return this;
        }
        public async Task<PythonExecutionContext> Run()
        {
            try
            {
                if (!this._skipRequirements)
                {
                    await this.EnsureRequirements();
                }
            }
            catch (Exception err)
            {

            }
            return this;
        }

        public void Dispose()
        {
            this.Process?.Dispose();
            this.serviceScope?.Dispose();
        }
    }
}
