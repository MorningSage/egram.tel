using System;
using TdLib;

namespace Tel.Egram.Services.Authentication;

public interface IAuthenticator
{
    IObservable<TdApi.AuthorizationState> ObserveState();
    IObservable<TdApi.Ok> SetupParameters();
    IObservable<TdApi.Ok> SetPhoneNumber(string phoneNumber);
    IObservable<TdApi.Ok> CheckCode(string code);
    IObservable<TdApi.Ok> CheckPassword(string password);
}