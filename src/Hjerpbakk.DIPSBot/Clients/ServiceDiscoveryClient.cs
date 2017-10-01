using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Hjerpbakk.DIPSBot.Clients
{
    public class ServiceDiscoveryClient
    {
        const string ContainerName = "discovery";
        const string KitchenServiceURLBlobName = "kitchen-service.txt";

        readonly Configuration configuration;

        readonly CloudBlobClient blobClient;
        readonly CloudBlobContainer discoveryContainer;

        public ServiceDiscoveryClient(Configuration configuration)
		{
            this.configuration = configuration;
			var storageAccount = CloudStorageAccount.Parse(configuration.ConnectionString);

			blobClient = storageAccount.CreateCloudBlobClient();

			discoveryContainer = blobClient.GetContainerReference(ContainerName);
			discoveryContainer.CreateIfNotExistsAsync().GetAwaiter();
        }

        public async Task SetKitchenServiceURL() {
            var blobRef = discoveryContainer.GetBlobReference(KitchenServiceURLBlobName);
            string ip = null;
            using (var memoryStream = new MemoryStream())
    	    {
    	       await blobRef.DownloadToStreamAsync(memoryStream);
    	       ip = Encoding.UTF8.GetString(memoryStream.ToArray());
    	    }

            if (ip == null) {
                throw new ArgumentException($"Could not find Kitchen Service IP in container {ContainerName} with blob name {KitchenServiceURLBlobName}");
            }

			configuration.KitchenServiceURL = "http://" + ip + "/";
        }

        public async Task UploadNewKitchenServiceURL(string url) {
            var blobRef = discoveryContainer.GetBlockBlobReference(KitchenServiceURLBlobName);
            await blobRef.UploadTextAsync(url);
		}
    }
}
