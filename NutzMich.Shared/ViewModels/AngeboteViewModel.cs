using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NutzMich.Shared.ViewModels
{
    class AngeboteViewModel
    {
        public ObservableCollection<AngebotViewModel> AlleAngebote { get; private set; }
        public ObservableCollection<AngebotViewModel> MeineAngebote { get; private set; }

        public AngeboteViewModel()
        {
            AlleAngebote = new ObservableCollection<AngebotViewModel>();
            MeineAngebote = new ObservableCollection<AngebotViewModel>();
        }
    }
}
