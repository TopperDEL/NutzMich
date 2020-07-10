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
using Uno.Extensions.Specialized;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace NutzMich.Shared.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class AngebotViewModel : INotifyPropertyChanged
    {
        private IThumbnailHelper _thumbnailHelper;
        public Angebot Angebot { get; set; }
        public ObservableCollection<ReservierungsZeitraumViewModel> Reservierungen { get; set; }

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

        public Verfuegbarkeit Verfuegbarkeit { get; private set; }

        public string VerfuegbarkeitsDetails { get; private set; }
        public Brush VerfuegbarkeitsAmpel{ get; private set; }

        public AngebotViewModel(Angebot angebot)
        {
            Verfuegbarkeit = Verfuegbarkeit.Unbekannt;
            VerfuegbarkeitsDetails = "";
            VerfuegbarkeitsAmpel = new SolidColorBrush(Colors.Black);

            _thumbnailHelper = Factory.GetThumbnailHelper();
            Barrel.ApplicationId = "nutzmich_monkeycache";

            SetIsNotLoading();
            Angebot = angebot;

            Fotos = new ObservableCollection<AttachmentImage>();
            Reservierungen = new ObservableCollection<ReservierungsZeitraumViewModel>();
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

        private bool _reservierungenGeladen;
        public async Task LoadReservierungenAsync(bool nachladenErzwingen = false)
        {
            if (_reservierungenGeladen && !nachladenErzwingen)
                return;

            var reservierungen = await Factory.GetReservierungService().GetReservierungenAsync(Angebot.AnbieterId, Angebot.Id);
            foreach(var reservierung in reservierungen.Where(r=>r.Bis >= DateTime.Now))
            {
                Reservierungen.Add(new ReservierungsZeitraumViewModel(reservierung));
            }

            RefreshVerfügbarkeit();

            _reservierungenGeladen = true;
        }

        private void RefreshVerfügbarkeit()
        {
            var reservierungHeute = Reservierungen.Where(r => r.Zeitraum.Von < DateTime.Now && DateTime.Now < r.Zeitraum.Bis);
            if (reservierungHeute.Count() == 0)
            {
                Verfuegbarkeit = Verfuegbarkeit.Verfuegbar;
                //Aktuell nicht verliehen
                var nächsteZukünftige = Reservierungen.Where(r => r.Zeitraum.Von > DateTime.Now);
                if(nächsteZukünftige.Count() != 0)
                {
                    VerfuegbarkeitsDetails = "Verfügbar bis " + nächsteZukünftige.OrderBy(r => r.Zeitraum.Von).First().Zeitraum.Bis.AddDays(-1).ToString("d");
                    VerfuegbarkeitsAmpel = new SolidColorBrush(Colors.Orange);
                }
                else
                {
                    VerfuegbarkeitsDetails = "";
                    VerfuegbarkeitsAmpel = new SolidColorBrush(Colors.DarkGreen);
                }
            }
            else
            {
                Verfuegbarkeit = Verfuegbarkeit.Verliehen;
                VerfuegbarkeitsDetails = "Verfügbar ab " + reservierungHeute.First().Zeitraum.Bis.AddDays(1).ToString("d");
                VerfuegbarkeitsAmpel = new SolidColorBrush(Colors.DarkRed);
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Verfuegbarkeit)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerfuegbarkeitsDetails)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VerfuegbarkeitsAmpel)));
        }
    }
}
