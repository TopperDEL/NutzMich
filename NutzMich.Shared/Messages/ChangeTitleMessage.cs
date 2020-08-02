using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Messages
{
    public class ChangeTitleMessage : ValueChangedMessage<string>
    {
        public ChangeTitleMessage(string newTitle) : base(newTitle)
        {
        }
    }
}
