using NutzMich.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace NutzMich.Shared.ViewModels
{
    class AngebotViewModel
    {
        public Angebot Angebot { get; set; }

        public BitmapImage Thumbnail { get; set; }
        public ObservableCollection<string> Fotos { get; private set; }

        public AngebotViewModel() : this(new Angebot())
        {

        }
        public AngebotViewModel(Angebot angebot)
        {
            Angebot = angebot;

            Thumbnail = new BitmapImage(new Uri(@"ms-appx:///Assets/ProductPlaceholder.jpg"));
            Fotos = new ObservableCollection<string>();
            Fotos.Add(@"ms-appx:///Assets/ProductPlaceholder.jpg");
            Fotos.Add(@"ms-appx:///Assets/ProductPlaceholder.jpg");
        }
    }
}
