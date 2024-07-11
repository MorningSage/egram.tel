using Microsoft.Extensions.DependencyInjection;
using Tel.Egram.ViewModels.Application;

namespace Tel.Egram.ViewModels.DependencyInjection;

public static class InjectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        
        // ToDo: Add all view models
        
        return services;
    }
}