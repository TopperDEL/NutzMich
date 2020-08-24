using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace NutzMich.Shared.Messages
{
    public class ChangePageMessage : ValueChangedMessage<Tuple<Page, string, bool>>
    {
        public ChangePageMessage(Page newPage, string newTitle, bool setTitleOnly = false) : base(new Tuple<Page, string, bool>(newPage, newTitle, setTitleOnly))
        {
        }
    }
}
