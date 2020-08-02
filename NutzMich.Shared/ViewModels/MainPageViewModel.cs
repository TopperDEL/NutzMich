using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutzMich.Shared.ViewModels
{
    public class MainPageViewModel : ObservableRecipient, IRecipient<ChangeTitleMessage>
    {
        private string title;
        public string Title { get => title; set => SetProperty(ref title, value); }

        public MainPageViewModel()
        {
        }

        public void Receive(ChangeTitleMessage message)
        {
            Title = message.Value;
        }
    }
}
