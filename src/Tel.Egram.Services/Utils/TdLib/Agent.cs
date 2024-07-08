using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using TdLib;

namespace Tel.Egram.Services.Utils.TdLib;

public class Agent(TdClient client) : IAgent
{
    public IObservable<TdApi.Update> Updates => Observable
        .FromEventPattern<TdApi.Update>(h => client.UpdateReceived += h, h => client.UpdateReceived -= h)
        .Select(a => a.EventArgs)
        .OfType<TdApi.Update>();

    public IObservable<T> Execute<T>(TdApi.Function<T> function) where T : TdApi.Object => client.ExecuteAsync(function).ToObservable();

    public IObservable<T> Execute<T>(TdApi.Function<T> function, TimeSpan timeout) where T : TdApi.Object
    {
        var delay = Task.Delay(timeout).ContinueWith<T>(_ => throw new TaskCanceledException("Execution timeout"));
        var task  = Task.WhenAny(delay, client.ExecuteAsync(function)).ContinueWith(t => t.Result.Result);

        return task.ToObservable();
    }

    public IObservable<T> Execute<T>(TdApi.Function<T> function, CancellationToken cancellationToken) where T : TdApi.Object
    {
        var delay = Task.Delay(Timeout.Infinite, cancellationToken).ContinueWith<T>(_ => throw new TaskCanceledException("Execution timeout"), cancellationToken);
        var task  = Task.WhenAny(delay, client.ExecuteAsync(function)).ContinueWith(t => t.Result.Result, cancellationToken);

        return task.ToObservable();
    }
}