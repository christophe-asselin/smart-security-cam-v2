using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;

namespace SmartSecurityCamV2
{
    class FaceRecognition
    {
        private static readonly string subscriptionKey = "2e1d9bb269cc4bc7982bf3da845fdbb5";
        private static readonly string uriBase = "https://canadacentral.api.cognitive.microsoft.com/face/v1.0/detect";
        private static readonly string requestParameters = "returnFaceId=true&returnFaceLandmarks=false";
        private static readonly string uri = uriBase + "?" + requestParameters;

        private readonly HttpClient client;

        public FaceRecognition()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

        }

        public async Task<string> MakeAnalysisRequestAsync(SoftwareBitmap softwareBitmap)
        {
            HttpResponseMessage response;

            byte[] byteData = GetImageAsByteArray(softwareBitmap);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream"); // "application/json"

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                return contentString;

                /*AnalysisReceivedEventArgs eventArg = new AnalysisReceivedEventArgs(contentString);

                EventHandler eventHandler = AnalysisReceived;
                eventHandler(this, eventArg);*/
            }

        }

        public static byte[] GetImageAsByteArray(SoftwareBitmap softwareBitmap)
        {
            byte[] byteData = new byte[4 * softwareBitmap.PixelHeight * softwareBitmap.PixelWidth];
            softwareBitmap.CopyToBuffer(byteData.AsBuffer());
            return byteData;
        }

        // public event EventHandler AnalysisReceived;

        /*public class AnalysisReceivedEventArgs : EventArgs
        {
            public AnalysisReceivedEventArgs(string analysis)
            {
                this.Analysis = analysis;
            }
            private string Analysis { get; set; }
        }*/
    }
}
