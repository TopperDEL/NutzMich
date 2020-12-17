using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Shared.Messages;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace NutzMich.Shared.ViewModels
{
    public delegate void CurrentPageChangedEventHandler(Page newPage);
    public delegate void NavigateToPageEventHandler(Type pageToNavigateTo, object parameters);

    [Bindable()]
    public class MainPageViewModel : ObservableRecipient, IRecipient<ChangePageMessage>, IRecipient<SetCommandsMessage>, IRecipient<NavigateMessage>
    {
        private string title;
        public string Title { get => title; set => SetProperty(ref title, value); }

        public ObservableCollection<NutzMichCommand> Commands { get; } = new ObservableCollection<NutzMichCommand>();

        private Page currentPage;
        public Page CurrentPage { get => currentPage; set => SetProperty(ref currentPage, value); }

        public bool IstEingeloggt { get; set; }

        public event CurrentPageChangedEventHandler CurrentPageChanged;
        public event NavigateToPageEventHandler NavigateToPage;

        public MainPageViewModel()
        {
            IstEingeloggt = Factory.GetLoginService().IsLoggedIn();
        }

        public void Receive(ChangePageMessage message)
        {
            if (message.Value.Item3 == false)
            {
                if (CurrentPage != message.Value.Item1)
                    CurrentPageChanged?.Invoke(message.Value.Item1);
                
                CurrentPage = message.Value.Item1;
            }

            Title = message.Value.Item2;
        }

        public void Receive(SetCommandsMessage message)
        {
            Commands.Clear();
            foreach(var command in message.Value)
            {
                if (command.NurWennAngemeldet && !IstEingeloggt)
                    continue;

                Commands.Add(command);
            }
        }

        public void Receive(NavigateMessage message)
        {
            NavigateToPage?.Invoke(message.Value.Item1, message.Value.Item2);
        }
    }
}
