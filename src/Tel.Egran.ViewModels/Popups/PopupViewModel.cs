using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tel.Egram.Model.Popups;
using Tel.Egram.Services.Popups;

namespace Tel.Egran.ViewModels.Popups;

public partial class PopupViewModel : AbstractViewModelBase
{
    private readonly IPopupController _popupController;

    [ObservableProperty] private PopupContext[] _popupList;
    [ObservableProperty] private int _selectedPopupIndex;
    [ObservableProperty] private bool _isVisible = true;
    
    public PopupViewModel(PopupContext popup, IPopupController popupController)
    {
        _popupController = popupController;
        _popupList = [ popup ];
        SelectedPopupIndex = 0;
    }
    
    [RelayCommand]
    private void CloseSelectedPopup()
    {
        // ToDo: Allow viewing multiple at once and pass the index/popup as a param?
        _popupController.Hide();
    }
}