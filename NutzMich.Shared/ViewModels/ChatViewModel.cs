using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NutzMich.Shared.ViewModels
{
    public delegate void ScrollToChatNachrichtEventHandler(ChatNachrichtViewModel newMessage);
    public class ChatViewModel:INotifyPropertyChanged
    {
        private IChatPollingService _pollingservice;
        private IChatService _chatService;
        private ILoginService _loginService;
        private CoreDispatcher _dispatcher;
        private Angebot _angebot;

        public event PropertyChangedEventHandler PropertyChanged;
        public event ScrollToChatNachrichtEventHandler ScrollToChatNachricht;

        public string NachrichtComposer { get; set; }
        public ObservableCollection<ChatNachrichtViewModel> Nachrichten { get; set; }

        public ChatViewModel(IChatPollingService pollingService, IChatService chatService, ILoginService loginService, Angebot angebot, CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Nachrichten = new ObservableCollection<ChatNachrichtViewModel>();

            _pollingservice = pollingService;
            _pollingservice.NachrichtErhalten += _pollingservice_NachrichtErhalten;

            _chatService = chatService;

            _loginService = loginService;

            _angebot = angebot;
            _pollingservice.StartPolling(angebot);
        }

        private async void _pollingservice_NachrichtErhalten(Angebot angebot, ChatNachricht nachricht)
        {
            if (angebot.Id != _angebot.Id)
                return;

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 var neueNachricht = new ChatNachrichtViewModel(nachricht) { IchBinEmpfaenger = true };
                 Nachrichten.Add(neueNachricht);
                 ScrollToChatNachricht?.Invoke(neueNachricht);
             });

            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nachrichten)));
            //Todo: buffer
        }

        public async Task SendNachricht()
        {
            var includeForeignAccess = Nachrichten.Count == 0;

            ChatNachricht nachricht = new ChatNachricht();
            nachricht.AngebotID = _angebot.Id;
            nachricht.EmpfaengerAnbieterID = _angebot.AnbieterId;
            nachricht.Nachricht = NachrichtComposer;
            nachricht.SendeDatum = DateTime.Now;
            nachricht.SenderAnbieterID = _loginService.AnbieterId;
            //await _chatService.SendNachrichtAsync(nachricht, _angebot.NachrichtenAccess, includeForeignAccess);

            var neueNachricht = new ChatNachrichtViewModel(nachricht) { IchWarSender = true };
            Nachrichten.Add(neueNachricht);
            NachrichtComposer = "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NachrichtComposer)));
            ScrollToChatNachricht?.Invoke(neueNachricht);
            //Todo: buffer
        }

        public void Cleanup()
        {
            _pollingservice.EndPolling(_angebot);
        }
    }
}
