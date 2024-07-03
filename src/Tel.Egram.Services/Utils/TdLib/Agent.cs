using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using TdLib;

namespace Tel.Egram.Services.Utils.TdLib
{
    public class Agent : IAgent
    {
        private readonly TdClient _client;

        public Agent(TdClient client)
        {
            _client = client;
        }

        public IObservable<TdApi.Update> Updates
        {
            get
            {
                return Observable.FromEventPattern<TdApi.Update>(h => _client.UpdateReceived += h, h => _client.UpdateReceived -= h)
                    .Select(a => a.EventArgs)
                    .OfType<TdApi.Update>();
            }
        }

        public IObservable<T> Execute<T>(TdApi.Function<T> function)
            where T : TdApi.Object
        {
            return _client.ExecuteAsync(function).ToObservable();
        }

        public IObservable<T> Execute<T>(TdApi.Function<T> function, TimeSpan timeout)
            where T : TdApi.Object
        {
            var delay = Task.Delay(timeout)
                .ContinueWith<T>(_ => throw new TaskCanceledException("Execution timeout"));
            
            var task = Task.WhenAny(delay, _client.ExecuteAsync(function))
                .ContinueWith(t => t.Result.Result);

            return task.ToObservable();
        }

        public IObservable<T> Execute<T>(TdApi.Function<T> function, CancellationToken cancellationToken)
            where T : TdApi.Object
        {
            var delay = Task.Delay(Timeout.Infinite, cancellationToken)
                .ContinueWith<T>(_ => throw new TaskCanceledException("Execution timeout"));
            
            var task = Task.WhenAny(delay, _client.ExecuteAsync(function))
                .ContinueWith(t => t.Result.Result);

            return task.ToObservable();
        }
    }
}