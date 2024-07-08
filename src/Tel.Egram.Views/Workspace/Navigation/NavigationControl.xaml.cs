using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Workspace.Navigation;

namespace Tel.Egram.Views.Workspace.Navigation;

public class NavigationControl : BaseControl<NavigationModel>
{
    public NavigationControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}