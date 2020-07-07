using NutzMich.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NutzMich.Shared.Resources
{
    public class ChatMessageTemplateSelector: DataTemplateSelector
    {
        public DataTemplate FromTemplate { get; set; }

        public DataTemplate ToTemplate { get; set; }
        public DataTemplate ReservierungTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) =>
            SelectTemplateCore(item);

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var message = (ChatNachrichtViewModel)item;
            if (!string.IsNullOrEmpty(message.Nachricht.TechnischerNachrichtenTyp) && message.Nachricht.TechnischerNachrichtenTyp == "Reservierung")
                return ReservierungTemplate;

            return message.IchWarSender ? FromTemplate : ToTemplate;
        }
    }
}
