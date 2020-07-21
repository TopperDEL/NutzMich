using NutzMich.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NutzMich.Shared.ViewModels
{
    [Bindable]
    public class AttachmentImageViewModel
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
    }
}
