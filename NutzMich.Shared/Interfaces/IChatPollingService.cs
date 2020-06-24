using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.Interfaces
{
    public delegate void NachrichtErhaltenDelegate(Angebot angebot, ChatNachricht nachricht);
    public interface IChatPollingService
    {
        void StartPolling(Angebot angebot);
        void EndPolling(Angebot angebot);

        event NachrichtErhaltenDelegate NachrichtErhalten;
    }
}
