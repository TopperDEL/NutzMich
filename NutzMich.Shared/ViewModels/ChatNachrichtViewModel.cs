using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.ViewModels
{
    public class ChatNachrichtViewModel
    {
        public ChatNachricht Nachricht { get; set; }
        public bool IchWarSender { get; set; }
        public bool IchBinEmpfaenger { get; set; }

        public ChatNachrichtViewModel(ChatNachricht nachricht)
        {
            Nachricht = nachricht;
        }
    }
}
