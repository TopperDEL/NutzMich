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
        public string TechnischerNachrichtenInhalt
        {
            get
            {
                if (Nachricht.TechnischerNachrichtenTyp == "Reservierung")
                    return Reservierung.GetChatMessageText(Nachricht.Nachricht);
                else
                    return Nachricht.TechnischerNachrichtenTyp;
            }
        }

        public ChatNachrichtViewModel(ChatNachricht nachricht)
        {
            Nachricht = nachricht;
        }
    }
}
