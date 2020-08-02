using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Shared.Messages;
using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NutzMich.Shared.ViewModels
{
    public class MainPageViewModel : ObservableRecipient, IRecipient<ChangeTitleMessage>, IRecipient<SetCommandsMessage>
    {
        private string title;
        public string Title { get => title; set => SetProperty(ref title, value); }

        public ObservableCollection<NutzMichCommand> Commands { get; } = new ObservableCollection<NutzMichCommand>();

        public MainPageViewModel()
        {
        }

        public void Receive(ChangeTitleMessage message)
        {
            Title = message.Value;
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
