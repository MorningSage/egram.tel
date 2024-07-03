using System;
using System.Reactive.Linq;
using TdLib;
using Tel.Egram.Services.Persistance;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services.Authentication
{
    public class Authenticator : IAuthenticator
    {
        private readonly IAgent _agent;
        private readonly IStorage _storage;

        public Authenticator(
            IAgent agent,
            IStorage storage
            )
        {
            _agent = agent;
            _storage = storage;
        }

        public IObservable<TdApi.AuthorizationState> ObserveState()
        {   
            return _agent.Updates.OfType<TdApi.Update.UpdateAuthorizationState>()
                .Select(update => update.AuthorizationState);
        }
        
        public IObservable<TdApi.Ok> SetupParameters()
        {
            // ToDo: Add to return value?
            _agent.Execute(new TdApi.SetOption
            {
                Name = "use_storage_optimizer",
                Value = new TdApi.OptionValue.OptionValueBoolean { Value = true }
            });
            
            _agent.Execute(new TdApi.SetOption
            {
                Name = "ignore_file_names",
                Value = new TdApi.OptionValue.OptionValueBoolean { Value = false }
            });
            
            return _agent.Execute(new TdApi.SetTdlibParameters
            {
                UseTestDc = false,
                DatabaseDirectory = _storage.TdLibDirectory,
                FilesDirectory = _storage.TdLibDirectory,
                UseFileDatabase = true,
                UseChatInfoDatabase = true,
                UseMessageDatabase = true,
                UseSecretChats = true,
                ApiId = 111112,
                ApiHash = new Guid(new byte[]
                    {142, 34, 97, 121, 94, 51, 206, 139, 4, 159, 245, 26, 236, 242, 11, 171}).ToString("N"),
                SystemLanguageCode = "en",
                DeviceModel = "Mac",
                SystemVersion = "0.1",
                ApplicationVersion = "0.1",
            });
        }

        public IObservable<TdApi.Ok> CheckEncryptionKey()
        {
            return _agent.Execute(new TdApi.CheckDatabaseEncryptionKey());
        }

        public IObservable<TdApi.Ok> SetPhoneNumber(string phoneNumber)
        {
            return _agent.Execute(new TdApi.SetAuthenticationPhoneNumber
                {
                    PhoneNumber = phoneNumber
                });
        }

        public IObservable<TdApi.Ok> CheckCode(string code, string firstName, string lastName)
        {
            return _agent.Execute(new TdApi.CheckAuthenticationCode
                {
                    Code = code,
                    FirstName = firstName,
                    LastName = lastName
                });
        }

        public IObservable<TdApi.Ok> CheckPassword(string password)
        {
            return _agent.Execute(new TdApi.CheckAuthenticationPassword
                {
                    Password = password
                });
        }
    }
}