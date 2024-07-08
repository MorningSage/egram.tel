using Avalonia.Markup.Xaml;
using Tel.Egram.Model.Authentication;
using Tel.Egran.ViewModels.Authentication;

namespace Tel.Egram.Views.Authentication;

public class AuthenticationControl : BaseControl<AuthenticationViewModel>
{
    public AuthenticationControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}