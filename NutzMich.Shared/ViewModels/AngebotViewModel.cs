using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace NutzMich.Shared.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class AngebotViewModel:INotifyPropertyChanged
    {
        public Angebot Angebot { get; set; }

        public BitmapImage Thumbnail { get; set; }
        public ObservableCollection<AttachmentImage> Fotos { get; private set; }

        public AngebotViewModel() : this(new Angebot())
        {

        }
        public AngebotViewModel(Angebot angebot)
        {
            Angebot = angebot;

            Thumbnail = new BitmapImage(new Uri(@"ms-appx:///Assets/ProductPlaceholder.jpg"));
            Fotos = new ObservableCollection<AttachmentImage>();
            LoadFirstImageAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task LoadFirstImageAsync()
        {
            var firstImage = await Factory.GetAngebotService().GetAngebotFirstImageAsync(Angebot);
            if (firstImage != null)
            {
                BitmapImage thumb = new BitmapImage();
                thumb.SetSource(firstImage);
                Thumbnail = thumb;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thumbnail)));
            }
        }
    }
}
