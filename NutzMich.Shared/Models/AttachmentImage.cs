using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace NutzMich.Shared.Models
{
    public class AttachmentImage
    {
        public Stream Stream { get; private set; }
        private BitmapImage _image = null;
        public BitmapImage Image
        {
            get
            {
                
                return _image;
            }
        }

        public AttachmentImage(Stream stream)
        {
            Stream = stream;
            if (_image == null)
            {
                _image = new BitmapImage();
#if WINDOWS_UWP
                _image.SetSourceAsync(Stream.AsRandomAccessStream());
#else
                    _image.SetSource(Stream);
#endif
            }
        }
    }
}
