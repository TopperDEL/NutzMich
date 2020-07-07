using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.Models
{
    public class ChatNachricht
    {
        public string Id { get; set; }
        public DateTime SendeDatum { get; set; }
        public string Nachricht { get; set; }
        public string AngebotID { get; set; }
        public string SenderAnbieterID { get; set; }
        public string EmpfaengerAnbieterID { get; set; }
        public string TechnischerNachrichtenTyp { get; set; }

        public ChatNachricht()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
