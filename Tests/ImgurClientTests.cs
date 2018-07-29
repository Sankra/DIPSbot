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
            var linkToImage = await imgurClient.UploadImage("https://maps.googleapis.com/maps/api/staticmap?size=600x600&scale=2&maptype=roadmap&region=no&markers=icon:https://hjerpbakk.com/assets/img/person.png%7CBeddingen%2010&markers=icon:https://hjerpbakk.com/assets/img/parking.png%3F1%7C63.435399485821,10.4099830985069&path=weight:5%7Ccolor:blue%7Cenc:_tdbK{wp~@yAvDGv@DjAJxABTGz@BhAJT&key=");

            Assert.StartsWith("https://i.imgur.com", linkToImage);
        }
    }
}
