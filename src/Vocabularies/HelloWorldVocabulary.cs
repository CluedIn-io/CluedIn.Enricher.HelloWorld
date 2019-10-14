using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;

namespace CluedIn.ExternalSearch.Providers.HelloWorld.Vocabularies
{
	public class HelloWorldVocabulary : SimpleVocabulary
	{
		public HelloWorldVocabulary()
		{
			VocabularyName = "HelloWorld User";
			KeyPrefix = "helloworld.user";
			KeySeparator = ".";
			Grouping = EntityType.Unknown;

			AddGroup("HelloWorld Details", group =>
			{
				Id = group.Add(new VocabularyKey("Id", VocabularyKeyDataType.Integer, VocabularyKeyVisibility.Visible));
				Name = group.Add(new VocabularyKey("Name", VocabularyKeyDataType.PersonName, VocabularyKeyVisibility.Visible));
				Username = group.Add(new VocabularyKey("Username", VocabularyKeyDataType.PersonName, VocabularyKeyVisibility.Visible));
				Email = group.Add(new VocabularyKey("Email", VocabularyKeyDataType.Email, VocabularyKeyVisibility.Hidden));
			});

		}

		public VocabularyKey Id { get; private set; }
		public VocabularyKey Name { get; private set; }
		public VocabularyKey Username { get; private set; }
		public VocabularyKey Email { get; private set; }
	}
}
