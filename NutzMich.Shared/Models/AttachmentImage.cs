using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace NutzMich.Shared.Models
{
    public class AttachmentImage:INotifyPropertyChanged
    {
        public Stream Stream { get; private set; }
        private BitmapImage _image = null;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public void ReplaceImage(Stream newStream)
        {
            Stream = newStream;
            _image = null;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
        }
    }
}
