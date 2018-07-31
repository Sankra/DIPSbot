using System.Net.Http;
using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
using Hjerpbakk.DIPSBot.Configuration;
using Tests.TestData;
using Xunit;

namespace Tests {
    public class ImgurClientTests {
        readonly ImgurClient imgurClient;

        public ImgurClientTests() {
            var configuration = CreateConfig.OfType<IImgurConfiguration>();
            imgurClient = new ImgurClient(configuration, new HttpClient());
        }

        [Fact]
        public async Task Upload() {
            var linkToImage = await imgurClient.UploadImage("https://hjerpbakk.com/img/git-remove-local-only-branches/git-remove-local-only-branches.png");

            Assert.StartsWith("https://i.imgur.com", linkToImage);
        }
    }
}
