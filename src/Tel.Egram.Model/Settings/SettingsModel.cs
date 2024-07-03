using PropertyChanged;
using ReactiveUI;

namespace Tel.Egram.Model.Settings
{
    [AddINotifyPropertyChangedInterface]
    public class SettingsModel : IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}