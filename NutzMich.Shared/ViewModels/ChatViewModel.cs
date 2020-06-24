using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace NutzMich.Shared.ViewModels
{
    public class ChatViewModel
    {
        private IChatPollingService _pollingservice;
        private Angebot _angebot;


        public ObservableCollection<ChatNachricht> Nachrichten { get; set; }

        public ChatViewModel(IChatPollingService pollingService, Angebot angebot)
        {
            _pollingservice = pollingService;
            _pollingservice.NachrichtErhalten += _pollingservice_NachrichtErhalten;

            _angebot = angebot;
            _pollingservice.StartPolling(angebot);
        }

        private void _pollingservice_NachrichtErhalten(Angebot angebot, ChatNachricht nachricht)
        {
            if (angebot.Id != _angebot.Id)
                return;

            Nachrichten.Add(nachricht);
        }
    }
}
