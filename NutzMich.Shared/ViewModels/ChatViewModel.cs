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
    public class ChatViewModel : INotifyPropertyChanged
    {
        private IChatPollingService _pollingservice;
        private IChatService _chatService;
        private ILoginService _loginService;
        private CoreDispatcher _dispatcher;
        public AngebotViewModel AngebotViewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event ScrollToChatNachrichtEventHandler ScrollToChatNachricht;

        public string NachrichtComposer { get; set; }
        public string NeuesteNachricht
        {
            get
            {
                if (Nachrichten == null || Nachrichten.Count == 0)
                    return "";
                else
                    return Nachrichten[Nachrichten.Count - 1].Nachricht.Nachricht.Substring(0, 200);
            }
        }
        public ObservableCollection<ChatNachrichtViewModel> Nachrichten { get; set; }

        public ChatViewModel(AngebotViewModel angebot)
        {
            AngebotViewModel = angebot;
        }

        public ChatViewModel(IChatPollingService pollingService, IChatService chatService, ILoginService loginService, Angebot angebot, CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            Nachrichten = new ObservableCollection<ChatNachrichtViewModel>();

            _pollingservice = pollingService;
            _pollingservice.NachrichtErhalten += _pollingservice_NachrichtErhalten;

            _chatService = chatService;

            _loginService = loginService;

            AngebotViewModel.Angebot = angebot;
            _pollingservice.StartPolling(angebot);
        }

        private async void _pollingservice_NachrichtErhalten(Angebot angebot, ChatNachricht nachricht)
        {
            if (angebot.Id != AngebotViewModel.Angebot.Id)
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
            nachricht.AngebotID = AngebotViewModel.Angebot.Id;
            nachricht.Nachricht = NachrichtComposer;
            nachricht.SendeDatum = DateTime.Now;
            if (AngebotViewModel.Angebot.AnbieterId == _loginService.AnbieterId)
            {
                nachricht.SenderAnbieterID = _loginService.AnbieterId;
                //nachricht.EmpfaengerAnbieterID = _angebot.AnbieterId;
                await _chatService.SendNachrichtAsync(nachricht, AngebotViewModel.Angebot.NachrichtenAccess, includeForeignAccess);
            }
            else
            {
                nachricht.EmpfaengerAnbieterID = AngebotViewModel.Angebot.AnbieterId;
                nachricht.SenderAnbieterID = _loginService.AnbieterId;
                await _chatService.SendNachrichtAsync(nachricht, AngebotViewModel.Angebot.NachrichtenAccess, includeForeignAccess);
            }

            var neueNachricht = new ChatNachrichtViewModel(nachricht) { IchWarSender = true };
            Nachrichten.Add(neueNachricht);
            NachrichtComposer = "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NachrichtComposer)));
            ScrollToChatNachricht?.Invoke(neueNachricht);
            //Todo: buffer
        }

        public void Cleanup()
        {
            _pollingservice.EndPolling(AngebotViewModel.Angebot);
        }
    }
}
