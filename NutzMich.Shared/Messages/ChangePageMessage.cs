using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace NutzMich.Shared.Messages
{
    public class ChangePageMessage : ValueChangedMessage<Tuple<Page, string>>
    {
        public ChangePageMessage(Page newPage, string newTitle) : base(new Tuple<Page, string>(newPage, newTitle))
        {
        }
    }
}
