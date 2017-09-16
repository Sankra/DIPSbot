using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hjerpbakk.DIPSbot.Services
{
	public interface IOrganizationService
	{
		Task<IEnumerable<string>> GetDevelopers();
	}
}