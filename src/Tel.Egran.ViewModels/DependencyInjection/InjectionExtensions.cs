using Microsoft.Extensions.DependencyInjection;
using Tel.Egran.ViewModels.Application;

namespace Tel.Egran.ViewModels.DependencyInjection;

public static class InjectionExtensions
{
    public static IServiceCollection AddViewModels(this IServiceCollection services)
    {
        services.AddSingleton<MainWindowViewModel>();
        
        
        
        return services;
    }
}