using PropertyChanged;
using ReactiveUI;

namespace Tel.Egran.ViewModels.Settings;

[AddINotifyPropertyChangedInterface]
public class SettingsModel : IActivatableViewModel
{
    public ViewModelActivator Activator { get; } = new();
}