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
                if(_image == null)
                {
                    _image = new BitmapImage();
                    _image.SetSource(Stream);
                }
                return _image;
            }
        }

        public AttachmentImage(Stream stream)
        {
            Stream = stream;
        }
    }
}
