using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Models
{
    public class ChatInfo
    {
        public string AngebotID { get; set; }
        public string NachrichtenAccess { get; set; }
        public string GegenseiteAnbieterID { get; set; }
        public List<ChatNachricht> Nachrichten { get; set; }

        public ChatInfo()
        {
            Nachrichten = new List<ChatNachricht>();
        }
    }
}
