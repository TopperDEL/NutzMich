using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NutzMich.Shared.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class AttachmentImageViewModel:INotifyPropertyChanged
    {
        public AttachmentImage AttachmentImage { get; set; }

        public Visibility KannNachVorne { get; set; }
        public Visibility KannNachHinten { get; set; }
        public Visibility IstSelektiert { get; set; }
        public Brush RahmenBrush { get; set; }

        public AttachmentImageViewModel(AttachmentImage attachmentImage)
        {
            AttachmentImage = attachmentImage;
            RahmenBrush = new SolidColorBrush(Colors.RoyalBlue);
            KannNachVorne = Visibility.Collapsed;
            KannNachHinten = Visibility.Collapsed;
            IstSelektiert = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RefreshBindings(bool imageChanged = false)
        {
            if(PropertyChanged!= null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(KannNachHinten)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(KannNachVorne)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IstSelektiert)));
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(RahmenBrush)));
                if(imageChanged)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(AttachmentImage)));
            }
        }
    }
}
