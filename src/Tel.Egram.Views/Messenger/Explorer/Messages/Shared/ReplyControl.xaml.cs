﻿using Avalonia.Markup.Xaml;
using Tel.Egram.Model.Messaging.Explorer.Messages;

namespace Tel.Egram.Views.Messenger.Explorer.Messages.Shared;

public class ReplyControl : BaseControl<ReplyModel>
{
    public ReplyControl()
    {
        AvaloniaXamlLoader.Load(this);
    }
}