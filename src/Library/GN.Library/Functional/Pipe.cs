using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Dynamic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GN.Library.Functional
{
    public class PipeContext : IServiceProvider, IDisposable
    {
        private IServiceScope _scope;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private TaskCompletionSource<object> _taskCompletionSource = new TaskCompletionSource<object>();
        public Task Task => _taskCompletionSource.Task;
        public Exception Error { private set; get; }
        public int BackoffFactor { private set; get; }
        public CancellationToken CancellationToken => cancellationTokenSource.Token;
        public ILogger Logger { get; private set; }
        public string Name { get; private set; }
        public int Trial { get; private set; }
        public int MaxTrials { get; private set; }
        public object Request { get; private set; }
        public object Result { get; set; }
        public ConcurrentDictionary<string, object> Parameters { get; private set; } = new ConcurrentDictionary<string, object>();
        public PipeContext WithServiceProvider(IServiceProvider serviceProvider)
        {
            this._scope = serviceProvider?.CreateScope();
            WithName(null);
            return this;
        }
        public PipeContext WithRequest(object request)
        {
            this.Request = request;
            return this;
        }
        public PipeContext SetResult(object result)
        {
            this.Result = result;
            if (!this.Task.IsCompleted && !this.Task.IsCanceled && !this.Task.IsFaulted)
                this._taskCompletionSource.SetResult(result);
            return this;
        }

        public PipeContext SetError(Exception err)
        {
            this.Error = err;
            if (!this.Task.IsCompleted && !this.Task.IsCanceled && !this.Task.IsFaulted)
                this._taskCompletionSource.SetException(err);
            return this;
        }

        public PipeContext WithName(string name)
        {
            this.Name = string.IsNullOrWhiteSpace(name) ? "noname" : name;
            this.Logger = this.GetService<ILoggerFactory>()?.CreateLogger(this.Name);
            return this;
        }
        public PipeContext WithCancelationToken(CancellationToken token)
        {
            this.cancellationTokenSource?.Dispose();
            this.cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            return this;
        }

        public object GetService(Type serviceType)
        {
            return this._scope?.ServiceProvider.GetService(serviceType);
        }
        public PipeContext WithTrials(int trials)
        {
            //this.Trial = trial;
            this.MaxTrials = trials;
            return this;
        }
        public PipeContext WithBackoffFactor(int factor)
        {
            this.BackoffFactor = factor;
            return this;
        }
        public void Cancel()
        {
            this.cancellationTokenSource.Cancel();
            if (!this.Task.IsCompleted && !this.Task.IsCanceled && !this.Task.IsFaulted)
                this._taskCompletionSource.SetCanceled();

        }

        public void Dispose()
        {
            this._scope?.Dispose();
            this.cancellationTokenSource?.Dispose();
        }

        internal void SetTrial(int trial, int maxtrials)
        {
            this.Trial = trial;
            this.MaxTrials = maxtrials;
        }
    }
    public class PipeContext<TC> : PipeContext where TC : PipeContext
    {
        public new TC WithServiceProvider(IServiceProvider serviceProvider) => base.WithServiceProvider(serviceProvider) as TC;
        public new TC WithRequest(object request) => base.WithRequest(request) as TC;
        public new TC SetResult(object result) => base.SetResult(result) as TC;
        public new TC WithName(string name) => base.WithName(name) as TC;
        public new TC WithCancelationToken(CancellationToken token) => base.WithCancelationToken(token) as TC;
        public new TC WithTrials(int trials) => base.WithTrials(trials) as TC;
        public new TC WithBackoffFactor(int factor) => base.WithBackoffFactor(factor) as TC;
    }

    public class PipeContext<TReq, TRes> : PipeContext
    {
        public new TReq Request { get; private set; }
        public new TRes Result { get; set; }

        public new PipeContext<TReq, TRes> WithServiceProvider(IServiceProvider serviceProvider) => base.WithServiceProvider(serviceProvider) as PipeContext<TReq, TRes>;
        public PipeContext<TReq, TRes> WithRequest(TReq request)
        {
            this.Request = request;
            return this;
        }
        public PipeContext<TReq, TRes> SetResult(TRes result)
        {
            this.Result = result;
            return this;
        }

        public new PipeContext<TReq, TRes> WithName(string name) => base.WithName(name) as PipeContext<TReq, TRes>;
        public new PipeContext<TReq, TRes> WithCancelationToken(CancellationToken token) => base.WithCancelationToken(token) as PipeContext<TReq, TRes>;
        public new PipeContext<TReq, TRes> WithTrials(int trials) => base.WithTrials(trials) as PipeContext<TReq, TRes>;
    }

    public class PipeContext<TC, TReq, TRes> : PipeContext<TReq, TRes> where TC : PipeContext<TReq, TRes>
    {
        public new TC WithServiceProvider(IServiceProvider serviceProvider) => base.WithServiceProvider(serviceProvider) as TC;
        public new TC WithRequest(TReq request) => base.WithRequest(request) as TC;
        public new TC SetResult(TRes result) => base.SetResult(result) as TC;
        public new TC WithName(string name) => base.WithName(name) as TC;
        public new TC WithCancelationToken(CancellationToken token) => base.WithCancelationToken(token) as TC;
        public new TC WithTrials(int trials) => base.WithTrials(trials) as TC;
        public new TC WithBackoffFactor(int factor) => base.WithBackoffFactor(factor) as TC;

    }
    public class Pipe<T>
    {
        private List<Func<T, Func<T, Task>, Task>> steps = new List<Func<T, Func<T, Task>, Task>>();
        private Func<T, Exception, int, Action, Task> except;
        private Func<T, Exception, Task> _finally;
        private int MaxTrials = 1;
        public Pipe<T> Then(Func<T, Func<T, Task>, Task> step)
        {
            this.steps.Add(step);
            return this;
        }
        public Pipe<T> Then(Func<T, Task> step)
        {
            this.steps.Add(async (x, n) =>
            {
                await step(x);
                await n(x);
            });
            return this;
        }
        public Pipe<T> Then(Action<T> step)
        {
            this.steps.Add((x, n) =>
            {
                step(x);
                return n(x);
            });
            return this;
        }
        public Pipe<T> Retrials(int count)
        {
            this.MaxTrials = count;
            return this;
        }
        public Pipe<T> Except(Func<T, Exception, int, Action, Task> except)
        {
            this.except = except;
            return this;
        }
        public Pipe<T> Except(Action<T, Exception, int, Action> except)
        {
            this.except = (a, b, c, d) =>
            {
                except(a, b, c, d);
                return Task.CompletedTask;
            };
            return this;
        }
        public Pipe<T> Except(Action<T, Exception, int> except)
        {
            this.except = (a, b, c, d) =>
            {
                except(a, b, c);
                return Task.CompletedTask;
            };
            return this;
        }


        public Pipe<T> Finally(Func<T, Exception, Task> finaly)
        {
            this._finally = finaly;
            return this;
        }
        public Pipe<T> Finally(Action<T, Exception> finaly)
        {
            this._finally = (a, b) =>
            {
                finaly?.Invoke(a, b);
                return Task.CompletedTask;
            };
            return this;
        }

        private async Task<T> DoRun(T context)
        {
            Task do_invoke(int idx)
            {
                if (idx == this.steps.Count)
                    return Task.CompletedTask;
                return this.steps[idx](context, ctx =>
                {
                    return do_invoke(idx + 1);
                });
            }
            await do_invoke(0);
            return context;
        }
        public async Task<T> Run(T context, int? trials = null, bool dispose = true)
        {
            Exception lastError = new Exception("");
            this.MaxTrials = trials.HasValue ? trials.Value : this.MaxTrials;
            this.MaxTrials = this.MaxTrials < 1 ? 1 : this.MaxTrials;
            var pipecontext = context as PipeContext;
            for (var i = 1; i <= this.MaxTrials; i++)
            {
                lastError = null;
                try
                {
                    if (context is PipeContext ctx)
                    {
                        ctx.SetTrial(i, this.MaxTrials);
                        ctx.CancellationToken.ThrowIfCancellationRequested();
                    }
                    context = await DoRun(context);
                    if (context is PipeContext __ctx)
                    {
                        __ctx.SetResult(__ctx.Result);
                    }
                    break;
                }

                catch (Exception err)
                {
                    lastError = err;
                    if (this.except != null)
                    {
                        var _ignore = false;
                        try
                        {
                            await (this.except(context, err, i, () =>
                            {
                                _ignore = true;

                            }));
                            if (_ignore)
                            {
                                lastError = null;
                                break;
                            }
                        }
                        catch (Exception _err)
                        {
                            lastError = _err;
                            break;
                        }
                    }
                    else if (i >= this.MaxTrials)
                    {
                        break;
                    }
                }
                if (pipecontext != null && pipecontext.BackoffFactor > 0)
                {
                    await Task.Delay(pipecontext.BackoffFactor * i * i * 1000, pipecontext.CancellationToken);
                }

            }
            this._finally?.Invoke(context, lastError);
            if (dispose && context is IDisposable disposable)
            {
                disposable.Dispose();
            }
            else if (dispose && context is IAsyncDisposable adisposable)
            {
                await adisposable.DisposeAsync();
            }
            if (lastError != null)
            {
                if (context is PipeContext _ctx)
                {
                    _ctx.SetError(lastError);
                }
                throw lastError;
            }
            return context;
        }

        public Task<T> Run(Action<T> configure, int? trials = null, bool dispose = true)
        {
            var context = Activator.CreateInstance<T>();
            configure?.Invoke(context);
            return Run(context, trials, dispose);
        }
        public T Start(Action<T> configure, int? trials = null, bool dispose = true)
        {
            var context = Activator.CreateInstance<T>();
            configure?.Invoke(context);
            _ = Run(context, trials, dispose);
            return context;
        }
        public T Start(T context, int? trials = null, bool dispose = true)
        {
            _ = Run(context, trials, dispose);
            return context;
        }
        public static Pipe<T> Setup(Action<Pipe<T>> configure = null)
        {
            var result = new Pipe<T>();
            configure?.Invoke(result);
            return result;
        }


    }

}
