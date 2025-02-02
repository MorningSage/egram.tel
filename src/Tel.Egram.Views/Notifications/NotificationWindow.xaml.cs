﻿using System.Reactive.Disposables;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Tel.Egram.Model.Notifications;
using Tel.Egran.ViewModels.Notifications;

namespace Tel.Egram.Views.Notifications;

public class NotificationWindow : WindowWithViewModel<NotificationViewModel>
{
    private static NotificationWindow _current;
        
    public NotificationWindow() //: base(false)
    {
        //this.WhenActivated(disposables =>
        //{
        //    this.BindAutohide()
        //        .DisposeWith(disposables);
        //});
            
        AvaloniaXamlLoader.Load(this);
    }

    public override void Show()
    {   
        _current?.Close();
            
        base.Show();

        _current = this;
    }

    protected override void OnClosed(EventArgs e)
    {
        _current = null;
            
        base.OnClosed(e);
    }
}