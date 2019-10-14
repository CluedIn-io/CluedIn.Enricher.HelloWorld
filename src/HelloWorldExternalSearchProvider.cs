using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using CluedIn.Core;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.HelloWorld.Client;
using CluedIn.ExternalSearch.Providers.HelloWorld.Models;
using CluedIn.ExternalSearch.Providers.HelloWorld.Vocabularies;
using RestSharp;
 
namespace CluedIn.ExternalSearch.Providers.HelloWorld
{
    /// <summary>The helloworld graph external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class HelloWorldExternalSearchProvider : ExternalSearchProviderBase
    {
	    private static readonly Guid ProviderId = Guid.Parse("2261b8f8-00b7-45bb-8112-5cc897fb16d8");

        private readonly IHelloWorldClient _client;

        public HelloWorldExternalSearchProvider(IHelloWorldClient client)
            : base(ProviderId, EntityType.Person)
        {
	        _client = client;
        }

        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
			if (!Accepts(request.EntityMetaData.EntityType))
				yield break;

			var entityType = request.EntityMetaData.EntityType;

			var id = request.QueryParameters.GetValue(HelloWorldVocabularies.User.Id, new HashSet<string>());

			var person = new Dictionary<string, string>
			{
				{ "id",           id.FirstOrDefault() }
			};

			if (person.Any())
				yield return new ExternalSearchQuery(this, entityType, person);
        }

		public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            var id = query.QueryParameters["id"].FirstOrDefault();
           
            if (string.IsNullOrEmpty(id))
                yield break;
 
            var user = _client.GetUser(id).Result;
			if (user != null ) 
				yield return new ExternalSearchQueryResult<User>(query, user);
        }

		public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
		{
			var resultItem = result.As<User>();

			var code = GetOriginEntityCode(resultItem);

			var clue = new Clue(code, context.Organization);

			PopulateMetadata(clue.Data.EntityData, resultItem);

			// If necessary, you can create multiple clues and return them.

			return new[] {clue};
		}

		public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<User>();
            return CreateMetadata(resultItem);
        }

        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return null;
        }
 
        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<User> resultItem)
        {
            var metadata = new EntityMetadataPart();
 
            PopulateMetadata(metadata, resultItem);
 
            return metadata;
        }

        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<User> resultItem)
        {
	        return new EntityCode(EntityType.Person, GetCodeOrigin(), resultItem.Data.id);
        }

        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("helloworld");
        }
 
        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<User> resultItem)
        {
            var code = GetOriginEntityCode(resultItem);
 
            metadata.EntityType       = EntityType.Person;
            metadata.Name             = resultItem.Data.name;
            metadata.OriginEntityCode = code;
 
            metadata.Codes.Add(code);
			metadata.Properties[HelloWorldVocabularies.User.Email] = resultItem.Data.email;
		}
    }
}