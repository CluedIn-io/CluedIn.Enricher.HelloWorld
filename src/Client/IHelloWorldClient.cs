using System.Threading.Tasks;
using CluedIn.ExternalSearch.Providers.HelloWorld.Models;

namespace CluedIn.ExternalSearch.Providers.HelloWorld.Client
{
	public interface IHelloWorldClient
	{
		Task<User> GetUser(string id);
	}
}