using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CluedIn.ExternalSearch.Providers.HelloWorld.Models;
using Newtonsoft.Json;
using RestSharp;

namespace CluedIn.ExternalSearch.Providers.HelloWorld.Client
{
	public class HelloWorldClient : IHelloWorldClient
	{
		private const string BaseUri = "https://jsonplaceholder.typicode.com";

		private readonly IRestClient _client;

		public HelloWorldClient(IRestClient client) 
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));

			client.BaseUrl = new Uri(BaseUri);
		}

		public async Task<User> GetUser(string id) => await GetAsync<User>($"users/{id}");

		private async Task<T> GetAsync<T>(string url)
		{
			var request = new RestRequest(url, Method.GET);

			var response = await _client.ExecuteTaskAsync(request);

			if (response.StatusCode != HttpStatusCode.OK)
			{
				var diagnosticMessage = $"Request to {_client.BaseUrl}{url} failed, response {response.ErrorMessage} ({response.StatusCode})";

				throw new InvalidOperationException($"Communication to jsonplaceholder unavailable. {diagnosticMessage}");
			}

			var data = JsonConvert.DeserializeObject<T>(response.Content);

			return data;
		}

	}
}
