using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using GN.Library.Xrm.Services;
using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GN.Library.MicroServices
{
    public class SolutionInfo
    {
        public byte[] Content { get; set; }
        public string UniqueName { get; set; }
        public Version Version { get; set; }
        public bool IsManaged { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UniqueName);
            }
        }

        public bool IsContentValid()
        {
            return this.Content != null && this.Content.Length > 10;
        }
        public override string ToString()
        {
            return $"{UniqueName} {Version}";
        }
    }
    public interface IMicroSolution : IMicroService
    {
        SolutionInfo GetSolutionInfo(bool refersh = false);
    }
    public class MicroSolutionBase : MicroServiceBase, IHealthCheck
    {
        protected SolutionInfo solutionInfo;
        protected string SolutionName;
        public class SolutionResource
        {
            public byte[] Content { get; set; }
            public string Name { get; set; }
            public Version Version { get; set; }
            public bool IsManaged { get; set; }
        }

        protected readonly ISolutionManager solutionManager;

        private readonly ILogger logger;
        public MicroSolutionBase(ISolutionManager solutionManager, ILogger logger = null)
            : base()
        {
            this.solutionManager = solutionManager;
            this.logger = logger;

        }

        public virtual SolutionInfo GetSolutionInfo(bool refersh = false)
        {
            if (solutionInfo == null || refersh)
            {
                this.solutionInfo = new SolutionInfo();
                var solution = this.ExtractSolutionInfoFromEmbededResources(SolutionName)
                    .OrderByDescending(x => x.Version)
                    .OrderByDescending(x => x.IsManaged)
                    .FirstOrDefault();
                if (solution != null)
                {
                    this.solutionInfo = new SolutionInfo
                    {
                        UniqueName = solution.Name,
                        Content = solution.Content,
                        Version = solution.Version,

                    };
                }

            }
            return this.solutionInfo;

        }
        private bool IsSolutionResourceName(string resourceName)
        {
            return !string.IsNullOrWhiteSpace(resourceName) &&
                resourceName.ToLowerInvariant().EndsWith(".zip");

        }
        private static bool WildCardMatch(string value, string pattern)
        {
            if (value == null || pattern == null)
                return false;
            var exp = "^" + Regex.Escape(pattern).Replace("\\?", ".").Replace("\\*", ".*") + "$";
            return Regex.IsMatch(value, exp);
        }
        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        protected IEnumerable<SolutionResource> ExtractSolutionInfoFromEmbededResources(string solutionName)
        {
            var result = new List<SolutionResource>();
            if (!string.IsNullOrWhiteSpace(solutionName))
            {
                var _solutionName = solutionName.ToLowerInvariant();
                var resourceNames = this.GetType().Assembly.GetManifestResourceNames()
                .Where(x => IsSolutionResourceName(x))
                .Where(x => WildCardMatch(x.ToLowerInvariant(), $"*{_solutionName.ToLowerInvariant()}*"))
                .ToList();
                foreach (var item in resourceNames)
                {
                    var parts = item.Split('.');
                    if (parts.Length > 1)
                    {
                        var name = parts[parts.Length - 2].ToLowerInvariant();
                        var version = name.Replace(_solutionName, "")
                            .Replace("unmanaged", "")
                            .Replace("managed", "")
                            .Replace("_", ".");
                        if (version.StartsWith("."))
                            version = version.Substring(1, version.Length - 1);
                        if (version.EndsWith("."))
                            version = version.Substring(0, version.Length - 1);
                        var ok = Version.TryParse(version, out var _v);
                        if (ok)
                        {
                            try
                            {
                                var strm = this.GetType().Assembly.GetManifestResourceStream(item);
                                result.Add(new SolutionResource
                                {
                                    Content = ReadFully(strm),
                                    Name = solutionName,
                                    IsManaged = item.ToLowerInvariant().Contains("managed"),
                                    Version = _v
                                });
                            }
                            catch { }
                        }
                    }
                }
            }



            return result;

        }

        public virtual async Task<bool> EsureSolution(CancellationToken cancellationToken)
        {
            var result = await Task.FromResult(false);

            var solution = this.GetSolutionInfo();
            if (solution.IsValid)
            {
                result = await this.solutionManager.EsureSolution(solution, cancellationToken);
            }

            return result;


        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            if (await this.EsureSolution(cancellationToken))
            {
                this.logger?.LogInformation
                    ("Solution '{0}' successfully identified. ", this.SolutionName);

            }
            await base.StartAsync(cancellationToken);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var name = $"Solution :{this.SolutionName}";
            if (!this.solutionInfo.IsValid)
            {
                return context.Unhealthy(this.solutionInfo.UniqueName).WriteLine("Solution is Not Valid");
            }
            if (await this.EsureSolution(cancellationToken))
            {
                return context.Healthy(name)
                    .WriteLine($"Verified Version: {this.solutionInfo.Version}");
            }
            else
            {
                return context.Unhealthy(name)
                    .WriteLine($"Solution is missing.");

            }
        }
    }
}
