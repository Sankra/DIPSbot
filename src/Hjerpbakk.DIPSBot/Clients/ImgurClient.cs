using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Configuration;
using Hjerpbakk.DIPSBot.Model.Imgur;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Clients {
    public class ImgurClient {
        readonly HttpClient httpClient;
        readonly string clientId;

        public ImgurClient(IImgurConfiguration imgurConfiguration, HttpClient httpClient) {
            clientId = imgurConfiguration.ImgurClientId;
            this.httpClient = httpClient;
        }

        public async Task<string> UploadImage(string imageUrl) {
            byte[] image = null;
            try {
                image = await httpClient.GetByteArrayAsync(imageUrl);
            } catch (Exception e) {
                throw new InvalidOperationException($"Could not download image from Google Maps: {e.Message}");
            }

            var content = new MultipartFormDataContent($"{DateTime.UtcNow.Ticks}") {
                    { new StringContent("file"), "type" },
                    { new ByteArrayContent(image), "image" }
                };
            var request = new HttpRequestMessage {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.imgur.com/3/image"),
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Client-ID", clientId);

            var result = await httpClient.SendAsync(request);
            result.EnsureSuccessStatusCode();
            string resultContent = await result.Content.ReadAsStringAsync();

            var uploadResult = JsonConvert.DeserializeObject<ImgurUploadResult>(resultContent);
            if (!uploadResult.Success) {
                throw new InvalidOperationException($"Could not upload image to Imgur. Returned status {uploadResult.Status}.");
            }

            return uploadResult.Data.Link.Replace("\\", string.Empty);
        }
    }
}
