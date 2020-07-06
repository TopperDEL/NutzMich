using NutzMich.Contracts.Interfaces;
using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace NutzMich.Shared.ViewModels
{
    public delegate void ScrollToChatNachrichtEventHandler(ChatNachrichtViewModel newMessage);
    public class ChatViewModel : INotifyPropertyChanged
    {
        internal static CoreDispatcher _coreDispatcher;
        private IChatPollingService _chatPollingService;
        private IChatBufferService _chatBufferService;
        private IChatService _chatService;
        private ILoginService _loginService;
        private ChatInfo _chatInfo;
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
                {
                    var nachricht = Nachrichten[Nachrichten.Count - 1].Nachricht.Nachricht;
                    if (nachricht.Length > 200)
                        return nachricht.Substring(0, 200) + "...";
                    else
                        return nachricht;
                }
            }
        }
        public ObservableCollection<ChatNachrichtViewModel> Nachrichten { get; set; }

        public bool Ungelesen
        {
            get
            {
                return _chatInfo.Ungelesen;
            }
        }

        public ChatViewModel(ChatInfo chatInfo, IChatPollingService chatPollingService, IChatService chatService, ILoginService loginService, IChatBufferService chatBufferService, Angebot angebot)
        {
            _chatPollingService = chatPollingService;
            _chatPollingService.NachrichtErhalten += _chatPollingService_NachrichtErhalten;
            _chatService = chatService;
            _loginService = loginService;
            _chatBufferService = chatBufferService;

            Nachrichten = new ObservableCollection<ChatNachrichtViewModel>(chatInfo.Nachrichten.Select(c => new ChatNachrichtViewModel(c) { IchBinEmpfaenger = _loginService.AnbieterId == c.EmpfaengerAnbieterID, IchWarSender = _loginService.AnbieterId == c.SenderAnbieterID }));

            AngebotViewModel = new AngebotViewModel(angebot);
            _chatPollingService.StartPolling(angebot);

            _chatInfo = chatInfo;
        }

        private async void _chatPollingService_NachrichtErhalten(Angebot angebot, ChatNachricht nachricht)
        {
            if (angebot.Id != AngebotViewModel.Angebot.Id || nachricht.SenderAnbieterID == _loginService.AnbieterId)
                return;

            await _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 var neueNachricht = new ChatNachrichtViewModel(nachricht) { IchBinEmpfaenger = true };
                 Nachrichten.Add(neueNachricht);
                 ScrollToChatNachricht?.Invoke(neueNachricht);
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ungelesen)));
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NeuesteNachricht)));
             });
        }

        public async Task SendNachricht()
        {
            var includeForeignAccess = Nachrichten.Count == 0;

            ChatNachricht nachricht = new ChatNachricht();
            nachricht.AngebotID = AngebotViewModel.Angebot.Id;
            nachricht.Nachricht = NachrichtComposer;
            nachricht.SendeDatum = DateTime.Now;
            nachricht.SenderAnbieterID = _loginService.AnbieterId;
            nachricht.EmpfaengerAnbieterID = _chatInfo.GegenseiteAnbieterID;
            await _chatService.SendNachrichtAsync(AngebotViewModel.Angebot, nachricht, _chatInfo.NachrichtenAccess, includeForeignAccess);

            var neueNachricht = new ChatNachrichtViewModel(nachricht) { IchWarSender = true };
            Nachrichten.Add(neueNachricht);
            NachrichtComposer = "";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NachrichtComposer)));
            ScrollToChatNachricht?.Invoke(neueNachricht);
        }

        public void Cleanup()
        {
            _chatPollingService.EndPolling(AngebotViewModel.Angebot);
        }

        public void RefreshBindings()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Nachrichten)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NachrichtComposer)));
        }

        public async Task SetGesehenAsync()
        {
            await _coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (_chatInfo.Ungelesen)
                {
                    _chatInfo.Ungelesen = false;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Ungelesen)));
                    _chatBufferService.PersistBuffer();
                }
            });
        }

        public string GetChatPartnerID()
        {
            return _chatInfo.GegenseiteAnbieterID;
        }
    }
}
