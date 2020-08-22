using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Pages;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Messages;
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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace NutzMich.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilEditPage : Page, INotifyPropertyChanged
    {
        public ProfilViewModel _profilVM;
        IProfilService _profilService;
        ILoginService _loginService;
        IThumbnailHelper _thumbnailHelper;

        public event PropertyChangedEventHandler PropertyChanged;

        public ProfilEditPage()
        {
            this.InitializeComponent();

            _profilService = Factory.GetProfilService();
            _loginService = Factory.GetLoginService();
            _thumbnailHelper = Factory.GetThumbnailHelper();
        }

        private void SetzeCommands()
        {
            Messenger.Default.Send(new ChangePageMessage(this, "Mein Profil"));
            Messenger.Default.Send(new SetCommandsMessage(new List<Models.NutzMichCommand>()
                {
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Back,
                        Command = Models.NutzMichCommand.GoBackCommand
                    },
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Save,
                        Command = new AsyncRelayCommand(SaveAsync),
                        NurWennAngemeldet = true
                    }
                }));
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SetzeCommands();

            Profil profil = await _profilService.GetProfilAsync(_loginService.AnbieterId);
            _profilVM = new ProfilViewModel(profil);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_profilVM)));
        }

        private async Task SaveAsync()
        {
            var saved = await _profilService.SetProfilAsync(_profilVM.Profil);
            if (saved)
                Models.NutzMichCommand.GoBackCommand.Execute(null);
            else
            {
                ContentDialog notSavedDlg = new ContentDialog()
                {
                    Title = "Fehler",
                    Content = "Profil konnte nicht gespeichert werden - bitte später noch einmal versuchen",
                    CloseButtonText = "Ok"
                };

                await notSavedDlg.ShowAsync();
            }
        }

        private async void PersonPicture_Click(object sender, RoutedEventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "NutzMich",
                Name = "ProfilFoto.jpg",
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (file == null)
                return;

            var fileRead = File.OpenRead(file.Path);
            _profilVM.Profil.ProfilbildBase64 = await _thumbnailHelper.ThumbnailToBase64Async(new Models.AttachmentImage(fileRead));

            _profilVM.RefreshProfilbild();
        }
    }
}
