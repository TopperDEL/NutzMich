using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace NutzMich.Shared.ViewModels
{
    class AngeboteViewModel : INotifyPropertyChanged
    {
        public bool Loading { get; set; }

        public ObservableCollection<AngebotViewModel> AlleAngebote { get; private set; }
        public ObservableCollection<AngebotViewModel> MeineAngebote { get; private set; }

        public AngeboteViewModel()
        {
            AlleAngebote = new ObservableCollection<AngebotViewModel>();
            MeineAngebote = new ObservableCollection<AngebotViewModel>();
        }

        public void SetLoading()
        {
            Loading = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
        }

        public void SetNotLoading()
        {
            Loading = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Loading)));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
