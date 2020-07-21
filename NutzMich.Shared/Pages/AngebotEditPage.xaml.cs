using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NutzMich.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AngebotEditPage : Page, INotifyPropertyChanged
    {
        private AngebotViewModel _angebotVM;
        private IAngebotService _angebotService;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Kategorie> MoeglicheKategorien
        {
            get
            {
                if (_angebotVM != null && _angebotVM.Angebot != null)
                    return Kategorie.Kategorien.Select(k => new Kategorie(k.Key, k.Value)).ToList();
                else
                    return new List<Kategorie>();
            }
        }

        public AngebotEditPage()
        {
            this.InitializeComponent();

            _angebotService = Factory.GetAngebotService();

            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator();
            AltLeft.Key = VirtualKey.Left;
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);
            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;
        }

        

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                _angebotVM = e.Parameter as AngebotViewModel;
                _angebotVM.SetIsLoading();
                await _angebotVM.LoadFotos();
            }
            else
                _angebotVM = new AngebotViewModel();

            this.DataContext = _angebotVM;
            _angebotVM.SetIsNotLoading();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoeglicheKategorien)));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private bool On_BackRequested()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            return false;
        }

        private void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            On_BackRequested();
            args.Handled = true;
        }

        private async void Save(object sender, RoutedEventArgs e)
        {
            var pruefErgebnis = _angebotService.IstAngebotFehlerhaft(_angebotVM.Angebot);
            if(pruefErgebnis.Item1)
            {
                ContentDialog notSavedDlg = new ContentDialog()
                {
                    Title = "Fehler",
                    Content = pruefErgebnis.Item2,
                    CloseButtonText = "Ok"
                };

                await notSavedDlg.ShowAsync();
                return;
            }
            _angebotVM.SetIsLoading();
            var saved = await _angebotService.SaveAngebotAsync(_angebotVM.Angebot, _angebotVM.Fotos.ToList());
            _angebotVM.SetIsNotLoading();

            if (saved)
            {
                _angebotService.Refresh();
                On_BackRequested();
            }
            else
            {
                ContentDialog notSavedDlg = new ContentDialog()
                {
                    Title = "Fehler",
                    Content = "Dein Angebot konnte nicht gespeichert werden. Bitte später noch einmal versuchen.",
                    CloseButtonText = "Ok"
                };

                await notSavedDlg.ShowAsync();
            }
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            On_BackRequested();
        }

        private async void AddPhoto(object sender, RoutedEventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "NutzMich",
                Name = "AngebotsFoto.jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (file == null)
                return;

            var fileRead = File.OpenRead(file.Path);
            _angebotVM.Fotos.Add(new Models.AttachmentImage(fileRead));
        }

        private void KategorieHinzufuegen(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            var ID = (chk.Tag as Kategorie).ID;
            if (!_angebotVM.Angebot.Kategorien.Contains(ID))
                _angebotVM.Angebot.Kategorien.Add(ID);
        }

        private void KategorieEntfernen(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            var ID = (chk.Tag as Kategorie).ID;
            if (_angebotVM.Angebot.Kategorien.Contains(ID))
                _angebotVM.Angebot.Kategorien.Remove(ID);
        }

        private void KategorieCheckBoxLoaded(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            var ID = (chk.Tag as Kategorie).ID;
            if (_angebotVM.Angebot.Kategorien.Contains(ID))
                chk.IsChecked = true;
            else
                chk.IsChecked = false;
        }
    }
}
