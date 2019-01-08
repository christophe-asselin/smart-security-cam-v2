using System;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SmartSecurityCamV2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MotionSensor motionSensor;
        private Camera camera;
        private FaceRecognition faceRecognition;

        public MainPage()
        {
            this.InitializeComponent();
            motionSensor = new MotionSensor();
            camera = new Camera();
            faceRecognition = new FaceRecognition();
            camera.PictureTaken += Camera_PictureTaken;
            motionSensor.Triggered += MotionSensor_Triggered;
            Unloaded += MainPage_Unloaded;
        }

        private async void MotionSensor_Triggered()
        {
            if (!Camera.IsBusy)
            {
                await camera.TakePicture();
            }
        }


        private async void Camera_PictureTaken(object sender, Camera.PictureTakenEventArgs arg)
        {
            var softwareBitmap = arg.Bitmap;

            string response = await faceRecognition.MakeAnalysisRequestAsync(arg.Bitmap);

            if (response != "[]")
            {
                await CloudStorage.SendImageAsync(softwareBitmap, arg.Time);

                if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
                        softwareBitmap.BitmapAlphaMode == BitmapAlphaMode.Straight)
                {
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    var source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(softwareBitmap);
                    ImageControl.Source = source;
                });
            }
        }
        
        private void MainPage_Unloaded(object sender, object arg)
        {
            motionSensor.Dispose();
            camera.Dispose();
        }
    }
}
