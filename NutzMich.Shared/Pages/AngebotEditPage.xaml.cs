﻿using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using NutzMich.Contracts.Interfaces;
using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Messages;
using NutzMich.Shared.Models;
using NutzMich.Shared.Services;
using NutzMich.Shared.ViewModels;
using Plugin.ImageEdit;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Uno.Extensions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
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
        }



        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool istNeuesAngebot = false;

            base.OnNavigatedTo(e);

            if (e.Parameter != null)
            {
                _angebotVM = e.Parameter as AngebotViewModel;
                _angebotVM.SetIsLoading();
                await _angebotVM.LoadAngebotsStatus();
                await _angebotVM.LoadFotos();
                Messenger.Default.Send(new ChangePageMessage(this, _angebotVM.Angebot.Ueberschrift));
            }
            else
            {
                _angebotVM = new AngebotViewModel();
                Messenger.Default.Send(new ChangePageMessage(this, "Neues Angebot"));
                istNeuesAngebot = true;
            }
            var aktivierenDeaktivieren = new NutzMichCommand();
            if(_angebotVM.IstInaktiv)
            {
                aktivierenDeaktivieren.Symbol = Symbol.Play;
                aktivierenDeaktivieren.Command = new AsyncRelayCommand(AktivierenAsync);
                aktivierenDeaktivieren.NurWennAngemeldet = true;
            }
            else
            {
                aktivierenDeaktivieren.Symbol = Symbol.Stop;
                aktivierenDeaktivieren.Command = new AsyncRelayCommand(DeaktivierenAsync);
                aktivierenDeaktivieren.NurWennAngemeldet = true;
            }
            if (istNeuesAngebot)
            {
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
            else
            {
                Messenger.Default.Send(new SetCommandsMessage(new List<Models.NutzMichCommand>()
                {
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Back,
                        Command = Models.NutzMichCommand.GoBackCommand
                    },
                    aktivierenDeaktivieren,
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Delete,
                        Command = new AsyncRelayCommand(DeleteAsync),
                        NurWennAngemeldet = true
                    },
                    new Models.NutzMichCommand()
                    {
                        Symbol = Symbol.Save,
                        Command = new AsyncRelayCommand(SaveAsync),
                        NurWennAngemeldet = true
                    }
                }));
            }

            this.DataContext = _angebotVM;
            _angebotVM.SetIsNotLoading();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MoeglicheKategorien)));
            MarkiereErstesFotoAlsGalleriebild();
        }

        private async Task DeleteAsync()
        {
            ContentDialog deleteDlg = new ContentDialog()
            {
                Title = "Wirklich löschen?",
                Content = "Willst Du dieses Angebot wirklich unwiderruflich löschen?",
                PrimaryButtonText = "Ja",
                SecondaryButtonText = "Nein",
            };

            var res = await deleteDlg.ShowAsync();
            _angebotVM.SetIsLoading();
            if (res == ContentDialogResult.Primary)
            {
                var erfolg = await _angebotService.DeleteAngebotAsync(_angebotVM.Angebot);
                if (erfolg)
                    Models.NutzMichCommand.GoBackCommand.Execute(null);
            }
            _angebotVM.SetIsNotLoading();
        }

        private async Task DeaktivierenAsync()
        {
            ContentDialog questionDlg = new ContentDialog()
            {
                Title = "Deaktivieren",
                Content = "Angebot wirlich deaktivieren? Es erscheint dann nicht mehr in den Suchergebnissen.",
                PrimaryButtonText = "Ja",
                SecondaryButtonText = "Nein"
            };

            var res = await questionDlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                _angebotVM.IstInaktiv = true;
                await SaveAsync();
            }
        }

        private async Task AktivierenAsync()
        {
            ContentDialog questionDlg = new ContentDialog()
            {
                Title = "Aktivieren",
                Content = "Angebot wirlich wieder aktivieren? Es erscheint dann wieder in den Suchergebnissen.",
                PrimaryButtonText = "Ja",
                SecondaryButtonText = "Nein"
            };

            var res = await questionDlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                _angebotVM.IstInaktiv = false;
                await SaveAsync();
            }
        }

        private async Task SaveAsync()
        {
            var pruefErgebnis = _angebotService.IstAngebotFehlerhaft(_angebotVM.Angebot);
            if (pruefErgebnis.Item1)
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
            var saved = await _angebotService.SaveAngebotAsync(_angebotVM.Angebot, _angebotVM.Fotos.Select(s => s.AttachmentImage).ToList(), !_angebotVM.IstInaktiv);
            _angebotVM.SetIsNotLoading();

            if (saved)
            {
                _angebotService.Refresh();
                Models.NutzMichCommand.GoBackCommand.Execute(null);
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

        private bool _addingFoto = false;
        private async void AddPhoto(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_addingFoto) //Temp
                    return;

                _addingFoto = true;

                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    _addingFoto = false;
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "NutzMich",
                    Name = "AngebotsFoto.jpg",
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });

                _addingFoto = false;

                if (file == null)
                    return;

                var fileRead = File.OpenRead(file.Path);
                _angebotVM.Fotos.Add(new AttachmentImageViewModel(new Models.AttachmentImage(fileRead)));

                MarkiereErstesFotoAlsGalleriebild();
            }
            catch(Exception ex)
            {
                ContentDialog errorDlg = new ContentDialog()
                {
                    Title = "Fehler",
                    Content = "Es gab einen Fehler: " + ex.Message,
                    CloseButtonText = "Ok"
                };

                await errorDlg.ShowAsync();
            }
        }

        private async void DeletePhoto(object sender, RoutedEventArgs e)
        {
            ContentDialog deleteDlg = new ContentDialog()
            {
                Title = "Löschen?",
                Content = "Dieses Foto wirklich löschen?",
                PrimaryButtonText = "Ja",
                SecondaryButtonText = "Nein"
            };

            var res = await deleteDlg.ShowAsync();
            if (res == ContentDialogResult.Primary)
            {
                var vm = (sender as Button).Tag as AttachmentImageViewModel;
                _angebotVM.Fotos.Remove(vm);
                //_angebotVM.GeloeschteFotos(vm.AttachmentImage.)
            }

            MarkiereErstesFotoAlsGalleriebild();
        }

        private void MarkiereErstesFotoAlsGalleriebild()
        {
            foreach (var foto in _angebotVM.Fotos)
            {
                if (_angebotVM.Fotos.IndexOf(foto) == 0)
                    foto.RahmenBrush = new SolidColorBrush(Colors.Orange);
                else
                    foto.RahmenBrush = new SolidColorBrush(Colors.Blue);

                foto.RefreshBindings();
            }
        }

        private void MovePhotoBack(object sender, RoutedEventArgs e)
        {
            var vm = (sender as Button).Tag as AttachmentImageViewModel;
            var currentIndex = _angebotVM.Fotos.IndexOf(vm);
            if (currentIndex != 0)
            {
                currentIndex--;
                _angebotVM.Fotos.Remove(vm);
                _angebotVM.Fotos.Insert(currentIndex, vm);
            }
            MarkiereErstesFotoAlsGalleriebild();
        }

        private void MovePhotoForward(object sender, RoutedEventArgs e)
        {
            var vm = (sender as Button).Tag as AttachmentImageViewModel;
            var currentIndex = _angebotVM.Fotos.IndexOf(vm);
            if (currentIndex != _angebotVM.Fotos.Count - 1)
            {
                currentIndex++;
                _angebotVM.Fotos.Remove(vm);
                _angebotVM.Fotos.Insert(currentIndex, vm);
            }
            MarkiereErstesFotoAlsGalleriebild();
        }

        private async void RotatePhoto(object sender, RoutedEventArgs e)
        {
            var vm = (sender as Button).Tag as AttachmentImageViewModel;
            vm.AttachmentImage.Stream.Position = 0;
            var mstream = vm.AttachmentImage.Stream.ToMemoryStream();
            var array = mstream.ToArray();
            var image = await CrossImageEdit.Current.CreateImageAsync(array);
            image.Rotate(90);
            vm.AttachmentImage.ReplaceImage(new MemoryStream(image.ToJpeg()));
            vm.RefreshBindings();
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

        private void Fotos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removed = e.RemovedItems.FirstOrDefault();
            if (removed != null && removed is AttachmentImageViewModel)
            {
                var vm = removed as AttachmentImageViewModel;
                vm.IstSelektiert = Visibility.Collapsed;
                vm.RefreshBindings();
            }

            var added = e.AddedItems.FirstOrDefault();
            if (added != null && added is AttachmentImageViewModel)
            {
                var vm = added as AttachmentImageViewModel;
                vm.IstSelektiert = Visibility.Visible;
                if (_angebotVM.Fotos.IndexOf(vm) == 0 || _angebotVM.Fotos.Count == 1)
                    vm.KannNachVorne = Visibility.Collapsed;
                else
                    vm.KannNachVorne = Visibility.Visible;

                if (_angebotVM.Fotos.IndexOf(vm) == _angebotVM.Fotos.Count - 1 || _angebotVM.Fotos.Count == 1)
                    vm.KannNachHinten = Visibility.Collapsed;
                else
                    vm.KannNachHinten = Visibility.Visible;

                vm.RefreshBindings();
            }
        }
    }
}
