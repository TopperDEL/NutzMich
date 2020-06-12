using NutzMich.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace NutzMich.Shared.ViewModels
{
    class AngebotViewModel
    {
        public Angebot Angebot { get; set; }

        public BitmapImage Thumbnail { get; set; }

        public AngebotViewModel(Angebot angebot)
        {
            Angebot = angebot;

            Thumbnail = new BitmapImage(new Uri(@"ms-appx:///Assets/ProductPlaceholder.jpg"));
        }
    }
}
