using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Messages
{
    public class SetCommandsMessage : ValueChangedMessage<List<NutzMichCommand>>
    {
        public SetCommandsMessage(List<NutzMichCommand> commands) : base(commands)
        {
        }
    }
}
