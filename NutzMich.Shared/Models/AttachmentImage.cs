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
                if (_image == null)
                {
                    _image = new BitmapImage();
#if WINDOWS_UWP
                _image.SetSourceAsync(Stream.AsRandomAccessStream());
#else
                    //Workaround für Uno. Das muss aber besser gehen.
                    byte[] data = new byte[Stream.Length];
                    Stream.Read(data, 0, (int)Stream.Length);
                    MemoryStream mstream = new MemoryStream(data);
                    _image.SetSource(mstream);
#endif
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
