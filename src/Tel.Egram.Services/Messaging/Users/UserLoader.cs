using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Messaging.Users;

public class UserLoader(IAgent agent) : IUserLoader
{
    public IObservable<User> GetMe() => agent.Execute(new TdApi.GetMe()).Select(user => new User
    {
        UserData = user
    });
}