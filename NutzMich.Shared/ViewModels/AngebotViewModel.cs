using MonkeyCache.FileStore;
using NutzMich.Contracts.Models;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using Plugin.Connectivity;
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
    public class AngebotViewModel : INotifyPropertyChanged
    {
        private IThumbnailHelper _thumbnailHelper;
        public Angebot Angebot { get; set; }

        public BitmapImage Thumbnail
        {
            get
            {
                if (!string.IsNullOrEmpty(Angebot.ThumbnailBase64))
                    return _thumbnailHelper.ThumbnailFromBase64(Angebot.ThumbnailBase64).Image;
                else
                    return new BitmapImage(new Uri(@"ms-appx:///Assets/ProductPlaceholder.jpg"));
            }
        }
        public ObservableCollection<AttachmentImage> Fotos { get; private set; }
        public bool Loading { get; set; }
        public bool NotLoading { get; set; }
        public string BeschreibungShort
        {
            get
            {
                if (Angebot?.Beschreibung?.Length > 80)
                    return Angebot.Beschreibung.Substring(0, 80) + "...";
                else
                    return Angebot?.Beschreibung;
            }
        }

        public AngebotViewModel() : this(new Angebot())
        {

        }

        public AngebotViewModel(Angebot angebot)
        {
            _thumbnailHelper = Factory.GetThumbnailHelper();
            Barrel.ApplicationId = "nutzmich_monkeycache";

            SetIsNotLoading();
            Angebot = angebot;

            Fotos = new ObservableCollection<AttachmentImage>();
        }

        public void SetIsLoading()
        {
            Loading = true;
            NotLoading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotLoading)));
        }

        public void SetIsNotLoading()
        {
            Loading = false;
            NotLoading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NotLoading)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task LoadFotos()
        {
            var images = await Factory.GetAngebotService().GetAngebotImagesAsync(Angebot);
            int count = 1;
            foreach (var image in images)
            {
                if (images != null)
                {
                    if (!Barrel.Current.IsExpired("angebot_foto_" + count + "_" + Angebot.Id) || !CrossConnectivity.Current.IsConnected)
                    {
                        var mstream = new MemoryStream(Barrel.Current.Get<byte[]>("angebot_foto_" + count + "_" + Angebot.Id));
                        Fotos.Add(new AttachmentImage(mstream));
                    }
                    else
                    {
                        byte[] data = new byte[image.Length];
                        image.Read(data, 0, (int)image.Length);

                        Fotos.Add(new AttachmentImage(image));
                        Barrel.Current.Add<byte[]>("angebot_foto_" + count + "_" + Angebot.Id, data, TimeSpan.FromDays(365));
                    }
                    count++;
                }
            }
        }
    }
}
