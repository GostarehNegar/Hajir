using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GostarehNegarBot.Internals
{
    //public class PipeUnrecoverableException : Exception
    //{
    //    public PipeUnrecoverableException(string message, Exception innerException = null) : base(message, innerException) { }
    //}
    //public class WithPipe
    //{
    //    public class Context
    //    {
    //        IDictionary<string, object> Parameters { get; } = new ConcurrentDictionary<string, object>();
    //        public Context(IDictionary<string,object> parameters)
    //        {
    //            this.Parameters = new ConcurrentDictionary<string, object>(parameters ?? new Dictionary<string, object>());
    //        }
    //    }
    //    IDictionary<string, object> Parameters { get; }
    //    protected int trial = 0;
    //    public int MaxTrials { get; protected set; }

    //}
    public class WithPipe<T>
    {
        public class PipeContext
        {
            public IServiceProvider ServiceProvider { get; private set; }
            public T Traget { get; private set; }
            public IDictionary<string,object> Parameters { get; set; }
            public PipeContext(T target, IServiceProvider serviceProvider, IDictionary<string, object> parameters)
            {
                this.Parameters = new ConcurrentDictionary<string, object>(parameters ?? new Dictionary<string, object>());
                this.Traget = target;
                this.ServiceProvider = serviceProvider;
            }
        }

        private Func<T, WithPipe<T>, Func<T, Task>, Task> pipe = (x, p, n) => n(x);
        private List<Func<T, WithPipe<T>, Func<T, Task>, Task>> steps = new List<Func<T, WithPipe<T>, Func<T, Task>, Task>>();
        private int maxTrials = 1;
        private ConcurrentDictionary<string, object> parameters = new ConcurrentDictionary<string, object>();
        private Func<T, Exception, WithPipe<T>, bool> except;
        public IDictionary<string, object> Parameters => this.parameters;

        public WithPipe<T> Then(Func<T, WithPipe<T>, Func<T, Task>, Task> step)
        {
            this.steps.Add(step);
            return this;
        }
        public WithPipe<T> Then(Func<T, Func<T, Task>, Task> step)
        {
            this.steps.Add((x, p, n) => step(x, n));
            return this;
        }
        public WithPipe<T> Then(Func<T, Task> step)
        {
            this.steps.Add(async (x, p, n) =>
            {
                await step(x);
                await n(x);

            });
            return this;
        }
        public WithPipe<T> Except(Func<T, Exception, WithPipe<T>, bool> except)
        {
            this.except = except;

            return this;
        }

        public WithPipe<T> Retrials(int count)
        {
            this.maxTrials = count;
            return this;
        }
        private async Task<T> DoRun(T context)
        {
            Task do_invoke(int idx)
            {
                if (idx == this.steps.Count)
                    return Task.CompletedTask;
                return this.steps[idx](context, this, ctx =>
                 {
                     return do_invoke(idx + 1);
                 });
            }
            await do_invoke(0);
            return context;
        }
        public async Task<T> Run(T context, int? trials = null)
        {
            Exception lastError = new Exception("");
            this.maxTrials = trials.HasValue ? trials.Value : this.maxTrials;
            this.maxTrials = this.maxTrials < 1 ? 1 : this.maxTrials;
            for (var i = 0; i < this.maxTrials; i++)
            {
                try
                {
                    return await DoRun(context);

                }
                catch (Exception err)
                {
                    lastError = err;
                    if (this.except != null && this.except(context, err, this))
                    {

                    }
                    //this.except?.Invoke(context, err);
                    else if (i == this.maxTrials - 1)
                    {
                        throw;
                    }
                }
            }
            throw lastError;
        }


        public static WithPipe<T> Setup(Action<WithPipe<T>> configure = null)
        {
            var result = new WithPipe<T>();
            configure?.Invoke(result);
            return result;
        }
        public static WithPipe<WithPipe<T>.PipeContext> SetupEx()
        {
            var result = new WithPipe<WithPipe<T>.PipeContext>();
            return result;
        }


    }
}
