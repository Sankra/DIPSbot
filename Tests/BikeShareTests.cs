using System.Threading.Tasks;
using Hjerpbakk.DIPSBot.Clients;
using Xunit;

namespace Tests {
    public class BikeShareTests {
        [Fact]
        public async Task Test1() {
            var client = new TrondheimBysykkelClient();

            var bikeShareStation = await client.FindNearesBikeSharingStation("Høyskoleveien");

            Assert.Equal("Gløshaugen", bikeShareStation.Name);
        }
    }
}
