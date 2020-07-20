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
using System.Diagnostics;

namespace NutzMich.Shared.Services
{
    public class ChatPollingService : IChatPollingService
    {
        internal static Semaphore getNachrichtenMutex;

        private IChatService _chatService;
        private Dictionary<Angebot, CancellationTokenSource> _pollingTasks;

        public event NachrichtErhaltenDelegate NachrichtErhalten;

        public ChatPollingService(IChatService chatService)
        {
            getNachrichtenMutex = new Semaphore(1,1);
            _chatService = chatService;
            _pollingTasks = new Dictionary<Angebot, CancellationTokenSource>();
        }

        public void StartPolling(Angebot angebot)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            if (!_pollingTasks.ContainsKey(angebot))
            {
                Task pollingTask = Task.Factory.StartNew(() => DoPollingAsync(angebot, source.Token), source.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                _pollingTasks.Add(angebot, source);
            }
        }

        public void EndPolling(Angebot angebot)
        {
            if (_pollingTasks.ContainsKey(angebot))
            {
                var pollingTask = _pollingTasks[angebot];
                if (pollingTask != null)
                {
                    pollingTask.Cancel();
                }
                _pollingTasks.Remove(angebot);
            }
        }

        private async Task DoPollingAsync(Angebot angebot, CancellationToken token)
        {
            List<ChatNachricht> nachrichten = new List<ChatNachricht>();
            while (!token.IsCancellationRequested)
            {
                getNachrichtenMutex.WaitOne();
                try
                {
                    var nachrichtenNeu = await _chatService.GetNachrichtenAsync(angebot, true);

                    if (nachrichtenNeu.Count() > 0)
                    {
                        foreach (var nachricht in nachrichtenNeu)
                        {
                            if (nachrichten.Where(n => n.Id == nachricht.Id).Count() == 0)
                            {
                                nachrichten.Add(nachricht);
                                NachrichtErhalten?.Invoke(angebot, nachricht);
                            }
                        }
                    }
                }
                catch 
                { 
                }
                finally
                {
                    getNachrichtenMutex.Release(1);
                }
                await Task.Delay(2000);
            }
        }
    }
}
