using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Authentication;

public class Authenticator(IAgent agent, IStorage storage) : IAuthenticator
{
    public IObservable<TdApi.AuthorizationState> ObserveState() => agent.Updates
        .OfType<TdApi.Update.UpdateAuthorizationState>()
        .Select(update => update.AuthorizationState);

    public IObservable<TdApi.Ok> SetupParameters()
    {
        // ToDo: Add to return value?
        agent.Execute(new TdApi.SetOption
        {
            Name = "use_storage_optimizer",
            Value = new TdApi.OptionValue.OptionValueBoolean { Value = true }
        });
            
        agent.Execute(new TdApi.SetOption
        {
            Name = "ignore_file_names",
            Value = new TdApi.OptionValue.OptionValueBoolean { Value = false }
        });
            
        return agent.Execute(new TdApi.SetTdlibParameters
        {
            UseTestDc = false,
            DatabaseDirectory = storage.TdLibDirectory,
            FilesDirectory = storage.TdLibDirectory,
            UseFileDatabase = true,
            UseChatInfoDatabase = true,
            UseMessageDatabase = true,
            UseSecretChats = true,
            ApiId = 111112,
            ApiHash = new Guid([ 142, 34, 97, 121, 94, 51, 206, 139, 4, 159, 245, 26, 236, 242, 11, 171 ]).ToString("N"),
            SystemLanguageCode = "en",
            DeviceModel = "Mac",
            SystemVersion = "0.1",
            ApplicationVersion = "0.1",
            // ToDo: we should add support for a database encryption key.  Note that this is no longer a separate call
            // DatabaseEncryptionKey = [] 
        });
    }

    public IObservable<TdApi.Ok> SetPhoneNumber(string phoneNumber) => agent.Execute(new TdApi.SetAuthenticationPhoneNumber
    {
        PhoneNumber = phoneNumber
    });

    public IObservable<TdApi.Ok> CheckCode(string code) => agent.Execute(new TdApi.CheckAuthenticationCode
    {
        Code = code
    });

    public IObservable<TdApi.Ok> CheckPassword(string password) => agent.Execute(new TdApi.CheckAuthenticationPassword
    {
        Password = password
    });
}