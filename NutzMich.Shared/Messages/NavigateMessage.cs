using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Messages
{
    public class NavigateMessage : ValueChangedMessage<Tuple<Type, object>>
    {
        public NavigateMessage(Type pageToNavigateTo, object parameter) : base(new Tuple<Type, object>(pageToNavigateTo, parameter))
        {
        }
    }
}
