using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;
using System.Linq;

namespace NutzMich.Shared.Services
{
    public class ChatPollingService : IChatPollingService
    {
        private IChatService _chatService;
        private Dictionary<Angebot, CancellationTokenSource> _pollingTasks;

        public event NachrichtErhaltenDelegate NachrichtErhalten;

        public ChatPollingService(IChatService chatService)
        {
            _chatService = chatService;
            _pollingTasks = new Dictionary<Angebot, CancellationTokenSource>();
        }

        public void StartPolling(Angebot angebot)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            Task pollingTask = Task.Run(() => DoPollingAsync(angebot, source.Token), source.Token);
            //pollingTask.Start();
            _pollingTasks.Add(angebot, source);
        }

        public void EndPolling(Angebot angebot)
        {
            var pollingTask = _pollingTasks[angebot];
            if(pollingTask != null)
            {
                pollingTask.Cancel();
            }
            _pollingTasks.Remove(angebot);
        }

        private async Task DoPollingAsync(Angebot angebot, CancellationToken token)
        {
            List<ChatNachricht> nachrichten = new List<ChatNachricht>();
            while (!token.IsCancellationRequested)
            {
                var nachrichtenNeu = await _chatService.GetNachrichtenAsync(angebot);
                if (nachrichtenNeu.Count() > nachrichten.Count())
                {
                    foreach(var nachricht in nachrichtenNeu)
                    {
                        if(nachrichten.Where(n=> n.Id == nachricht.Id).Count() == 0)
                        {
                            nachrichten.Add(nachricht);
                            NachrichtErhalten?.Invoke(angebot, nachricht);
                        }
                    }
                }
                await Task.Delay(1000);
            }
        }
    }
}
