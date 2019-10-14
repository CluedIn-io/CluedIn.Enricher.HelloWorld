using CluedIn.ExternalSearch.Providers.HelloWorld;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Messages.Processing;
using CluedIn.Core.Processing;
using CluedIn.Core.Serialization;
using CluedIn.Core.Workflows;
using CluedIn.ExternalSearch;
using CluedIn.ExternalSearch.Providers.HelloWorld.Client;
using CluedIn.ExternalSearch.Providers.HelloWorld.Installers;
using CluedIn.ExternalSearch.Providers.HelloWorld.Vocabularies;
using CluedIn.Testing.Base.Context;
using CluedIn.Testing.Base.ExternalSearch;
using CluedIn.Testing.Base.Processing.Actors;
using Moq;
using Xunit;

namespace ExternalSearch.HelloWorld.Integration.Tests
{
	public class HelloWorldTests 
	{
		private readonly TestContext _testContext;

		public HelloWorldTests()
		{
			_testContext = new TestContext();		
		}

		[Fact]
		public void Test()
		{
			//testContext.Container.Install(new InstallComponents());
			
			var properties = new EntityMetadataPart();
			properties.Properties.Add(HelloWorldVocabularies.User.Id, "1");


			// Arrange
				
			_testContext.Container.Install(new InstallComponents());

			IEntityMetadata entityMetadata = new EntityMetadataPart()
			{
				Name = "jsonplaceholder",
				EntityType = EntityType.Person,
				Properties = properties.Properties
			};


			//var clues = new List<CompressedClue>();

			//_testContext.ProcessingHub.Setup(h => h.SendCommand(It.IsAny<ProcessClueCommand>())).Callback<IProcessingCommand>(c => clues.Add(((ProcessClueCommand)c).Clue));

			_testContext.Container.Register(Component.For<IExternalSearchProvider>().UsingFactoryMethod(() => new HelloWorldExternalSearchProvider(_testContext.Container.Resolve<IHelloWorldClient>())));


			var processingContext = _testContext.Context.ToProcessingContext();
			var command = new ExternalSearchCommand();
			var processingAccessor = new ExternalSearchProcessingAccessor(processingContext.ApplicationContext);
			var commandWorkflow = new Workflow(processingContext, new EmptyWorkflowTemplate<ExternalSearchCommand>());
			
			command.With(processingContext);
			command.OrganizationId = processingContext.Organization.Id;
			command.EntityMetaData = entityMetadata;
			command.Workflow = commandWorkflow;
			processingContext.Workflow = command.Workflow;

			var stepResult = processingAccessor.ProcessWorkflowStep(processingContext, command);
			Assert.Equal(WorkflowStepResult.Repeat.SaveResult, stepResult.SaveResult);

			stepResult = processingAccessor.ProcessWorkflowStep(processingContext, command);
			Assert.Equal(WorkflowStepResult.Ignored.SaveResult, stepResult.SaveResult);
			//processingContext.Workflow.AddStepResult(result);

			//processingContext.Workflow.ProcessStepResult(processingContext, command);

			//// Assert
			//testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);

			//Assert.True(clues.Count > 0);
		}
	}
}
