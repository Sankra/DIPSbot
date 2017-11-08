using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hjerpbakk.DIPSbot.Services
{
	public class FileOrganizationService : IOrganizationService
	{
		public async Task<IEnumerable<string>> GetDevelopers()
		{
			using (var reader = File.OpenText("/Users/sankra/Desktop/developerss.txt"))
			{
				var fileContent = await reader.ReadToEndAsync();
				var lines = fileContent.Split(';');
				return from developer in lines
					   let mailPosition = developer.IndexOf('@')
					   select new[] { developer[mailPosition - 3], developer[mailPosition - 2], developer[mailPosition - 1] }
					   into usernameChars
					   select new string(usernameChars);
			}
		}
	}
}