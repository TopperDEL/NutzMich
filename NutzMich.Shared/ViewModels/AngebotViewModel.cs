using NutzMich.Contracts.Models;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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

        public async Task LoadFotos()
        {
            var firstImage = await Factory.GetAngebotService().GetAngebotFirstImageAsync(Angebot);
            Fotos.Add(new AttachmentImage(firstImage));
        }

        private async Task LoadFirstImageAsync()
        {
            var firstImage = await Factory.GetAngebotService().GetAngebotFirstImageAsync(Angebot);
            if (firstImage != null)
            {
                BitmapImage thumb = new BitmapImage();
#if WINDOWS_UWP
                await thumb.SetSourceAsync(firstImage.AsRandomAccessStream());
#else
                thumb.SetSource(firstImage);
#endif

                Thumbnail = thumb;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Thumbnail)));
                //Fotos.Add(new AttachmentImage(firstImage));
            }
        }
    }
}
