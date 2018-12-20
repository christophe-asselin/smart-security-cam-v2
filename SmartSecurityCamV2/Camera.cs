using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartSecurityCamV2
{
    class Camera
    {
        private MediaCapture mediaCapture;
        public static bool IsBusy { get; set; }

        public Camera()
        {
            IsBusy = false;
            Init();
        }

        private async void Init()
        {
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();
            mediaCapture.Failed += MediaCapture_Failed;
        }

        public async Task TakePicture()
        {
            IsBusy = true;

            var lowLagCapture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(
                ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));

            var capturedPhoto = await lowLagCapture.CaptureAsync();
            var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

            await lowLagCapture.FinishAsync();

            PictureTakenEventArgs arg = new PictureTakenEventArgs(softwareBitmap);
            OnPictureTaken(arg);

            IsBusy = false;
        }

        protected virtual void OnPictureTaken(PictureTakenEventArgs arg)
        {
            PictureTaken?.Invoke(this, arg);
        }

        public event EventHandler<PictureTakenEventArgs> PictureTaken;

        public class PictureTakenEventArgs : EventArgs
        {
            public PictureTakenEventArgs(SoftwareBitmap bitmap)
            {
                Bitmap = bitmap;
            }
            public SoftwareBitmap Bitmap { get; set; }
        }

        private void MediaCapture_Failed(object sender, object arg)
        {
            Debug.WriteLine("Capture Failed");
        }

        public void Dispose()
        {
            mediaCapture.Dispose();
        }
    }
}
