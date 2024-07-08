using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TdLib;
using Tel.Egram.Services.Authentication;
using Tel.Egram.Services.Graphics;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Mappers;
using Tel.Egram.Services.Messaging.Mappers.BasicMessages;
using Tel.Egram.Services.Messaging.Mappers.NoteMessages;
using Tel.Egram.Services.Messaging.Mappers.SpecialMessages;
using Tel.Egram.Services.Messaging.Mappers.VisualMessages;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Notifications;
using Tel.Egram.Services.Persistence;
using Tel.Egram.Services.Popups;
using Tel.Egram.Services.Settings;
using Tel.Egram.Services.Utils.Formatting;
using Tel.Egram.Services.Utils.Platforms;
using Tel.Egram.Services.Utils.TdLib;

namespace Tel.Egram.Services;

public static class Registry
{
    public static IServiceProvider Services { get; } = CollectServices();

    private static ServiceProvider CollectServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddUtils();
        serviceCollection.AddTdLib();
        serviceCollection.AddPersistence();
        serviceCollection.AddServices();

        serviceCollection.AddComponents();
        serviceCollection.AddAuthentication();
        serviceCollection.AddWorkspace();
        serviceCollection.AddSettings();
        serviceCollection.AddMessenger();

        return serviceCollection.BuildServiceProvider();
    }

    private static void AddUtils(this IServiceCollection services)
    {
        services.AddSingleton<IPlatform>(_ => Platform.GetPlatform());
        services.AddSingleton<IStringFormatter, StringFormatter>();
    }

    private static void AddTdLib(this IServiceCollection services)
    {
        services.AddSingleton<TdClient>(provider =>
        {
            var client = new TdClient();

            client.Execute(new TdApi.SetLogStream
            {
                LogStream = new TdApi.LogStream.LogStreamFile
                {
                    Path        = Path.Combine(provider.GetRequiredService<IStorage>().LogDirectory, "tdlib.log"),
                    MaxFileSize = 1_000_000, // 1 MiB
                }
            });

            client.SetLogVerbosityLevelAsync(5);

            return client;
        });

        services.AddSingleton<IAgent, Agent>();
    }

    private static void AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<IResourceManager, ResourceManager>();
        services.AddSingleton<IStorage, Storage>();
        services.AddSingleton<IFileLoader, FileLoader>();
        services.AddSingleton<IFileExplorer, FileExplorer>();
        services.AddSingleton<IDatabaseContextFactory, DatabaseContextFactory>();
        services.AddSingleton<DatabaseContext>(provider => provider.GetRequiredService<IDatabaseContextFactory>().CreateDbContext());
        services.AddSingleton<IKeyValueStorage, KeyValueStorage>();
    }

    private static void AddServices(this IServiceCollection services)
    {
        // graphics
        services.AddSingleton<IColorMapper, ColorMapper>();
        services.AddSingleton<IBitmapLoader, BitmapLoader>();

        // avatars
        services.AddSingleton<IAvatarCache>(_ => new AvatarCache(new MemoryCache(Options.Create(new MemoryCacheOptions { SizeLimit = 128 /* maximum 128 cached bitmaps */ }))));
        services.AddSingleton<IAvatarLoader, AvatarLoader>();

        // previews
        services.AddSingleton<IPreviewCache>(_ => new PreviewCache(new MemoryCache(Options.Create(new MemoryCacheOptions { SizeLimit = 16 /* maximum 16 cached bitmaps */ }))));
        services.AddSingleton<IPreviewLoader, PreviewLoader>();

        // chats
        services.AddSingleton<IChatLoader, ChatLoader>();
        services.AddSingleton<IChatUpdater, ChatUpdater>();
        services.AddSingleton<IFeedLoader, FeedLoader>();

        // messages
        services.AddSingleton<IMessageLoader, MessageLoader>();
        services.AddSingleton<IMessageSender, MessageSender>();

        // notifications
        services.AddSingleton<INotificationSource, NotificationSource>();

        // users
        services.AddSingleton<IUserLoader, UserLoader>();

        // auth
        services.AddSingleton<IAuthenticator, Authenticator>();

        // settings
        services.AddSingleton<IProxyManager, ProxyManager>();
    }

    private static void AddComponents(this IServiceCollection services)
    {
        services.AddSingleton<INotificationController, NotificationController>();
        services.AddSingleton<IPopupController, PopupController>();
    }

    private static void AddAuthentication(this IServiceCollection _)
    {
        // ToDo...
    }

    private static void AddMessenger(this IServiceCollection services)
    {   
        // messenger
        services.AddSingleton<IBasicMessageModelFactory,   BasicMessageModelFactory>();
        services.AddSingleton<INoteMessageModelFactory,    NoteMessageModelFactory>();
        services.AddSingleton<ISpecialMessageModelFactory, SpecialMessageModelFactory>();
        services.AddSingleton<IVisualMessageModelFactory,  VisualMessageModelFactory>();
        services.AddSingleton<IMessageModelFactory,        MessageModelFactory>();
    }

    private static void AddSettings(this IServiceCollection _)
    {
        // ToDo...
    }

    private static void AddWorkspace(this IServiceCollection _)
    {
        // ToDo...
    }
}