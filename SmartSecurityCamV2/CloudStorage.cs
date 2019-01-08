using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace SmartSecurityCamV2
{
    class CloudStorage
    {
        private static readonly string IOT_HUB_URI = "christophe-asselin-iot-hub.azure-devices.net";
        private static readonly string DEVICE_KEY = "9+2uycUmN/b+mgtLCMGvYvMGtqKSDagcyw1+ytOX76k=";
        private static readonly string DEVICE_ID = "MyDevice";
        private static DeviceClient deviceClient;

        public static async Task SendImageAsync(SoftwareBitmap bitmap, DateTime captureTime)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            using (InMemoryRandomAccessStream imageStream = new InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, imageStream);

                encoder.SetSoftwareBitmap(bitmap);
                encoder.IsThumbnailGenerated = true;

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception err)
                {
                    const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);
                    switch (err.HResult)
                    {
                        case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                            // If the encoder does not support writing a thumbnail, then try again
                            // but disable thumbnail generation.
                            encoder.IsThumbnailGenerated = false;
                            break;
                        default:
                            throw;
                    }
                }

                if (encoder.IsThumbnailGenerated == false)
                {
                    await encoder.FlushAsync();
                }

                string fileName = captureTime.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss") + ".jpg";

                await deviceClient.UploadToBlobAsync(fileName, imageStream.AsStream());
            }
        }
    }
}
