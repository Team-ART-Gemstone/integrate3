using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.IO;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Generic;

namespace CognitiveServisesVisionLibrary
{
    public class CognitiveVisionHelper
    {
        string _subscriptionKey;
        private const int numberOfCharsInOperationId = 36;
        public CognitiveVisionHelper()
        {
            // Replace this with real subscription key when building
            _subscriptionKey = "fa5bac0cc70e4eaabb60bcaa773c78cf";
        }
        public ComputerVisionClient GetVisionServiceClient()
        {
            ComputerVisionClient computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(_subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });
            computerVision.Endpoint = "https://eastus.api.cognitive.microsoft.com";
            return computerVision;
        }
        public async Task<TextOperationResult> start(byte[] photoBuffer)
        {
            var computerVision = GetVisionServiceClient();
            Stream imageStream = new MemoryStream(photoBuffer);
            return await ExtractLocalHandTextAsync(computerVision, imageStream);
        }
        private async Task<TextOperationResult> ExtractLocalHandTextAsync(
            ComputerVisionClient computerVision, Stream imageStream)
        {
            //byte[] photoBufferArray = photoBuffer.ToArray();
            //Stream imageStream = new MemoryStream(photoBuffer.ToArray());
            using (imageStream)
            {
                // Start the async process to recognize the text
                RecognizeTextInStreamHeaders textHeaders =
                    await computerVision.RecognizeTextInStreamAsync(
                        imageStream, TextRecognitionMode.Handwritten);
                return await GetTextAsync(computerVision, textHeaders.OperationLocation);
            }
        }
        private async Task<TextOperationResult> GetTextAsync(
            ComputerVisionClient computerVision, string operationLocation)
        {
            // Retrieve the URI where the recognized text will be
            // stored from the Operation-Location header
            string operationId = operationLocation.Substring(
                operationLocation.Length - numberOfCharsInOperationId);
            // Console.WriteLine("\nCalling GetHandwritingRecognitionOperationResultAsync()");
            TextOperationResult result =
                await computerVision.GetTextOperationResultAsync(operationId);
            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                //Console.WriteLine(
                //   "Server status: {0}, waiting {1} seconds...", result.Status, i);
                await Task.Delay(1000);
                result = await computerVision.GetTextOperationResultAsync(operationId);
            }
            return result;
        }
        public string ExtractOcr(TextOperationResult result)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var lines = result.RecognitionResult.Lines;
            foreach (Line line in lines)
            {
                stringBuilder.Append(line.Text);
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
    }
}