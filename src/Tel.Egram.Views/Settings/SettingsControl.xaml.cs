using Avalonia.Markup.Xaml;
using Tel.Egran.ViewModels.Settings;

namespace Tel.Egram.Views.Settings;

public class SettingsControl : BaseControl<SettingsModel>
{
    public SettingsControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}