using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Shared.Messages;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace NutzMich.Shared.ViewModels
{
    public class MainPageViewModel : ObservableRecipient, IRecipient<ChangePageMessage>, IRecipient<SetCommandsMessage>
    {
        private string title;
        public string Title { get => title; set => SetProperty(ref title, value); }

        public ObservableCollection<NutzMichCommand> Commands { get; } = new ObservableCollection<NutzMichCommand>();

        private Page currentPage;
        public Page CurrentPage { get => currentPage; set => SetProperty(ref currentPage, value); }

        public MainPageViewModel()
        {
        }

        public void Receive(ChangePageMessage message)
        {
            CurrentPage = message.Value.Item1;
            Title = message.Value.Item2;
        }

        public void Receive(SetCommandsMessage message)
        {
            Commands.Clear();
            foreach(var command in message.Value)
            {
                Commands.Add(command);
            }
        }
    }
}
