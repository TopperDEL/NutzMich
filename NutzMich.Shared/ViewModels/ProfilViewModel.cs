using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace NutzMich.Shared.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ProfilViewModel:INotifyPropertyChanged
    {
        private IThumbnailHelper _thumbnailHelper;

        public event PropertyChangedEventHandler PropertyChanged;

        public Profil Profil{ get; private set; }

        public BitmapImage Profilbild
        {
            get
            {
                if (!string.IsNullOrEmpty(Profil.ProfilbildBase64))
                    return _thumbnailHelper.ThumbnailFromBase64(Profil.ProfilbildBase64).Image;
                else
                    return new BitmapImage(new Uri(@"ms-appx:///Assets/ProfilePlaceholder.png"));
            }
        }

        public Visibility HatUeberMich { get { return string.IsNullOrEmpty(Profil.UeberMich) ? Visibility.Collapsed : Visibility.Visible; } }
        public Visibility HatProfilbild { get { return string.IsNullOrEmpty(Profil.ProfilbildBase64) ? Visibility.Collapsed : Visibility.Visible; } }

        public ProfilViewModel(Profil profil)
        {
            Profil = profil;
            _thumbnailHelper = Factory.GetThumbnailHelper();
        }

        public void RefreshProfilbild()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Profilbild)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HatProfilbild)));
        }
    }
}
