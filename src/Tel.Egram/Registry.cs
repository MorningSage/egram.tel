using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Splat;
using TdLib;
using Tel.Egram.Application;
using Tel.Egram.Model.Messenger.Explorer.Factories;
using Tel.Egram.Model.Notifications;
using Tel.Egram.Model.Popups;
using Tel.Egram.Services.Authentication;
using Tel.Egram.Services.Graphics;
using Tel.Egram.Services.Graphics.Avatars;
using Tel.Egram.Services.Graphics.Previews;
using Tel.Egram.Services.Messaging.Chats;
using Tel.Egram.Services.Messaging.Messages;
using Tel.Egram.Services.Messaging.Notifications;
using Tel.Egram.Services.Messaging.Users;
using Tel.Egram.Services.Persistance;
using Tel.Egram.Services.Settings;
using Tel.Egram.Services.Utils.Formatting;
using Tel.Egram.Services.Utils.Platforms;
using Tel.Egram.Services.Utils.TdLib;
using IBitmapLoader = Tel.Egram.Services.Graphics.IBitmapLoader;
using BitmapLoader = Tel.Egram.Services.Graphics.BitmapLoader;

namespace Tel.Egram;

public static class Registry
{
    private static readonly IReadonlyDependencyResolver CurrentResolver = Locator.Current;
        
    public static void AddUtils(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<IPlatform>(Platform.GetPlatform);
        services.RegisterLazySingleton<IStringFormatter>(() => new StringFormatter());
    }
        
    public static void AddTdLib(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton(() =>
        {
            var storage = CurrentResolver.GetService<IStorage>();
            var client = new TdClient();

            client.Execute(new TdApi.SetLogStream
            {
                LogStream = new TdApi.LogStream.LogStreamFile
                {
                    Path        = Path.Combine(storage.LogDirectory, "tdlib.log"),
                    MaxFileSize = 1_000_000, // 1 MiB
                }
            });
                
            client.SetLogVerbosityLevelAsync(5);
                
            return client;
        });

        services.RegisterLazySingleton<IAgent>(() =>
        {
            var client = CurrentResolver.GetService<TdClient>();
            return new Agent(client);
        });
    }
        
    public static void AddPersistance(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<IResourceManager>(
            () => new ResourceManager());
            
        services.RegisterLazySingleton<IStorage>(() => new Storage());
            
        services.RegisterLazySingleton<IFileLoader>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new FileLoader(agent);
        });
            
        services.RegisterLazySingleton<IFileExplorer>(() =>
        {
            var platform = CurrentResolver.GetService<IPlatform>();
            return new FileExplorer(platform);
        });
            
        services.RegisterLazySingleton<IDatabaseContextFactory>(() => new DatabaseContextFactory());
            
        services.RegisterLazySingleton(() =>
        {
            var factory = CurrentResolver.GetService<IDatabaseContextFactory>();
            return factory.CreateDbContext();
        });
            
        services.RegisterLazySingleton<IKeyValueStorage>(() =>
        {
            var db = CurrentResolver.GetService<DatabaseContext>();
            return new KeyValueStorage(db);
        });
    }

    public static void AddServices(this IMutableDependencyResolver services)
    {
        // graphics
        services.RegisterLazySingleton<IColorMapper>(() => new ColorMapper());
            
        services.RegisterLazySingleton<IBitmapLoader>(() =>
        {
            var fileLoader = CurrentResolver.GetService<IFileLoader>();
            return new BitmapLoader(fileLoader);
        });
            
        // avatars
        services.RegisterLazySingleton<IAvatarCache>(() =>
        {
            var options = Options.Create(new MemoryCacheOptions
            {
                SizeLimit = 128 // maximum 128 cached bitmaps
            });
            return new AvatarCache(new MemoryCache(options));
        });
            
        services.RegisterLazySingleton<IAvatarLoader>(() => new AvatarLoader(
            CurrentResolver.GetService<IPlatform>(),
            CurrentResolver.GetService<IStorage>(),
            CurrentResolver.GetService<IFileLoader>(),
            CurrentResolver.GetService<IAvatarCache>(),
            CurrentResolver.GetService<IColorMapper>()
        ));
            
        // previews
        services.RegisterLazySingleton<IPreviewCache>(() =>
        {
            var options = Options.Create(new MemoryCacheOptions
            {
                SizeLimit = 16 // maximum 16 cached bitmaps
            });
            return new PreviewCache(new MemoryCache(options));
        });
            
        services.RegisterLazySingleton<IPreviewLoader>(() =>
        {
            return new PreviewLoader(
                CurrentResolver.GetService<IFileLoader>(),
                CurrentResolver.GetService<IPreviewCache>()
            );
        });
            
        // chats
        services.RegisterLazySingleton<IChatLoader>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new ChatLoader(agent);
        });
            
        services.RegisterLazySingleton<IChatUpdater>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new ChatUpdater(agent);
        });
            
        services.RegisterLazySingleton<IFeedLoader>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new FeedLoader(agent);
        });
            
        // messages
        services.RegisterLazySingleton<IMessageLoader>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new MessageLoader(agent);
        });
        services.RegisterLazySingleton<IMessageSender>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new MessageSender(agent);
        });
            
        // notifications
        services.RegisterLazySingleton<INotificationSource>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new NotificationSource(agent);
        });
            
        // users
        services.RegisterLazySingleton<IUserLoader>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new UserLoader(agent);
        });
            
        // auth
        services.RegisterLazySingleton<IAuthenticator>(() =>
        {
            return new Authenticator(
                CurrentResolver.GetService<IAgent>(),
                CurrentResolver.GetService<IStorage>()
            );
        });
            
        // settings
        services.RegisterLazySingleton<IProxyManager>(() =>
        {
            var agent = CurrentResolver.GetService<IAgent>();
            return new ProxyManager(agent);
        });
    }

    public static void AddComponents(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<INotificationController>(() => new NotificationController());
        services.RegisterLazySingleton<IPopupController>(() => new PopupController());
    }
        
    public static void AddApplication(this IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton(() =>
        {
            var application = new MainApplication();
                
            application.Initializing += (sender, args) =>
            {
                var db = CurrentResolver.GetService<DatabaseContext>();
                db.Database.Migrate();
            };

            application.Disposing += async (sender, args) =>
            {
                var hub = CurrentResolver.GetService<TdClient>();
                await hub.DestroyAsync();
            };
                
            return application;
        });
    }
        
    public static void AddAuthentication(this IMutableDependencyResolver services)
    {
        //
    }
        
    public static void AddMessenger(this IMutableDependencyResolver services)
    {   
        // messenger
        services.RegisterLazySingleton<IBasicMessageModelFactory>(() => new BasicMessageModelFactory());
        services.RegisterLazySingleton<INoteMessageModelFactory>(() => new NoteMessageModelFactory());
        services.RegisterLazySingleton<ISpecialMessageModelFactory>(() => new SpecialMessageModelFactory(CurrentResolver.GetService<IStringFormatter>()));
        services.RegisterLazySingleton<IVisualMessageModelFactory>(() => new VisualMessageModelFactory());
            
        services.RegisterLazySingleton<IMessageModelFactory>(() => new MessageModelFactory(
            CurrentResolver.GetService<IBasicMessageModelFactory>(),
            CurrentResolver.GetService<INoteMessageModelFactory>(),
            CurrentResolver.GetService<ISpecialMessageModelFactory>(),
            CurrentResolver.GetService<IVisualMessageModelFactory>(),
            CurrentResolver.GetService<IStringFormatter>()
        ));
    }
        
    public static void AddSettings(this IMutableDependencyResolver services)
    {
        //
    }
        
    public static void AddWorkspace(this IMutableDependencyResolver services)
    {
        //
    }
}