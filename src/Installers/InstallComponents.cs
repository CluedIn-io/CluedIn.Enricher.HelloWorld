using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CluedIn.Core;
using CluedIn.ExternalSearch.Providers.HelloWorld.Client;
using RestSharp;

namespace CluedIn.ExternalSearch.Providers.HelloWorld.Installers
{
	public class InstallComponents : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container
				.AddFacilityIfNotExists<TypedFactoryFacility>()
				.Register(
					Component.For<IHelloWorldClient, HelloWorldClient>()
						.LifestyleTransient()
						.OnlyNewServices());

			if (!container.Kernel.HasComponent(typeof(IRestClient)) && !container.Kernel.HasComponent(typeof(RestClient)))
			{
				container.Register(Component.For<IRestClient, RestClient>());
			}
		}
	}
}
