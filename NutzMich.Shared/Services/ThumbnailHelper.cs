using NutzMich.Shared.Interfaces;
using NutzMich.Shared.Models;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace NutzMich.Shared.Services
{
    class ThumbnailHelper : IThumbnailHelper
    {
        public AttachmentImage ThumbnailFromBase64(string serialisedThumbnail)
        {
            var bytes = Convert.FromBase64String(serialisedThumbnail);
            AttachmentImage image = new AttachmentImage(new MemoryStream(bytes));
            return image;
        }

        public async Task<string> ThumbnailToBase64Async(AttachmentImage image)
        {
#if __ANDROID__
            var imageBytes = new byte[image.Stream.Length];
            image.Stream.Position = 0;
            image.Stream.Read(imageBytes, 0, (int)image.Stream.Length);
            var thumbnailBytes = GetThumbnailBytes(imageBytes, 100, 100);
#endif
#if WINDOWS_UWP
            StorageFolder folder = ApplicationData.Current.TemporaryFolder;
            var file = await folder.CreateFileAsync(Guid.NewGuid().ToString());
            var stream = await file.OpenStreamForWriteAsync();
            await image.Stream.CopyToAsync(stream);

            var thumb = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            var thumbnailBytes = new byte[thumb.Size];
            thumb.AsStream().Read(thumbnailBytes, 0, (int)thumb.Size);
#endif

            return Convert.ToBase64String(thumbnailBytes);
        }

#if __IOS__
//https://forums.xamarin.com/discussion/67531/how-to-resize-image-file-on-xamarin-forms-writeablebitmap-package-cant-be-added
        public byte[] GetThumbnailBytes(byte[] imageData, float width, float height)
        {

            UIKit.UIImage originalImage = ImageFromByteArray(imageData);

            var originalHeight = originalImage.Size.Height;
            var originalWidth = originalImage.Size.Width;

            nfloat newHeight = 0;
            nfloat newWidth = 0;

            if (originalHeight > originalWidth)
            {
                newHeight = height;
                nfloat ratio = originalHeight / height;
                newWidth = originalWidth / ratio;
            }
            else
            {
                newWidth = width;
                nfloat ratio = originalWidth / width;
                newHeight = originalHeight / ratio;
            }

            width = (float)newWidth;
            height = (float)newHeight;

            UIGraphics.BeginImageContext(new SizeF(width, height));
            originalImage.Draw(new RectangleF(0, 0, width, height));
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var bytesImagen = resizedImage.AsJPEG().ToArray();
            resizedImage.Dispose();
            return bytesImagen;
        }
#endif

#if __ANDROID__
        public byte[] GetThumbnailBytes(byte[] imageData, float width, float height)
        {
            // Load the bitmap 
            Android.Graphics.BitmapFactory.Options options = new Android.Graphics.BitmapFactory.Options();// Create object of bitmapfactory's option method for further option use
            options.InPurgeable = true; // inPurgeable is used to free up memory while required
            Android.Graphics.Bitmap originalImage = Android.Graphics.BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

            float newHeight = 0;
            float newWidth = 0;

            var originalHeight = originalImage.Height;
            var originalWidth = originalImage.Width;

            if (originalHeight > originalWidth)
            {
                newHeight = height;
                float ratio = originalHeight / height;
                newWidth = originalWidth / ratio;
            }
            else
            {
                newWidth = width;
                float ratio = originalWidth / width;
                newHeight = originalHeight / ratio;
            }

            Android.Graphics.Bitmap resizedImage = Android.Graphics.Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, true);

            originalImage.Recycle();

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);

                resizedImage.Recycle();

                return ms.ToArray();
            }
        }
#endif
    }
}
