using System;
using Newtonsoft.Json;

namespace Hjerpbakk.DIPSBot.Configuration
{
    public class AppConfiguration : IReadOnlyAppConfiguration
    {
        public string SlackAPIToken { get; set; }
        public string AdminUser { get; set;  }
        public string BotUser { get; set; }
		 
        public string KitchenServiceURL { get; set; }

		public string BlobStorageAccessKey { get; set; }
		public string BlobStorageAccountName { get; set; }
		public string BlobStorageEndpointSuffix { get; set; }

		[JsonIgnore]
		public string ConnectionString =>
			$"DefaultEndpointsProtocol=https;AccountName={BlobStorageAccountName};AccountKey={BlobStorageAccessKey};EndpointSuffix={BlobStorageEndpointSuffix}";

        public Action<Exception> FatalExceptionHandler { get; set; }
    }
}
