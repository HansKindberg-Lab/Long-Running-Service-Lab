using System;
using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Models;
using Service.Models.Builder.Extensions;
using Service.Models.Data;

namespace IntegrationTests.Models.Data
{
	[TestClass]
	public class DatabaseOperationRepositoryTest
	{
		#region Methods

		protected internal virtual async Task<DatabaseOperationRepository> CreateDatabaseOperationRepositoryAsync()
		{
			return await this.CreateDatabaseOperationRepositoryAsync(Mock.Of<ILoggerFactory>(), Mock.Of<IOperationContextFactory>());
		}

		protected internal virtual async Task<DatabaseOperationRepository> CreateDatabaseOperationRepositoryAsync(ILoggerFactory loggerFactory, IOperationContextFactory operationContextFactory)
		{
			return await Task.FromResult(new DatabaseOperationRepository(loggerFactory, operationContextFactory));
		}

		[TestMethod]
		public async Task General_Sqlite_Test()
		{
			await this.General_Test(Global.Configuration);
		}

		[TestMethod]
		public async Task General_SqlServer_Test()
		{
			await this.General_Test(Global.CreateConfiguration("appsettings.json", "appsettings.SqlServer.json"));
		}

		protected internal virtual async Task General_Test(IConfiguration configuration)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			var services = Global.CreateServices(configuration);

			using(var serviceProvider = services.BuildServiceProvider())
			{
				var applicationBuilder = new ApplicationBuilder(serviceProvider);
				applicationBuilder.UseOperationRepository();

				var databaseOperationRepository = (DatabaseOperationRepository)serviceProvider.GetRequiredService<IOperationRepository>();

				Assert.IsFalse((await databaseOperationRepository.ListAsync()).Any());

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		[ClassInitialize]
		public static async Task InitializeAsync(TestContext _)
		{
			await DatabaseHelper.DeleteDatabasesAsync();
		}

		[TestMethod]
		public async Task ToEntityAsync_ShouldReturnAnEntityWithCorrectResultType()
		{
			var operation = new Operation
			{
				Id = Guid.Empty,
				Result = new ProblemDetails { Title = "Test", Type = "Error" },
				Start = new DateTimeOffset(new DateTime(2000, 1, 1)),
			};

			var databaseOperationRepository = await this.CreateDatabaseOperationRepositoryAsync();

			var entity = await databaseOperationRepository.ToEntityAsync(operation);

			Assert.AreEqual("Microsoft.AspNetCore.Mvc.ProblemDetails, Microsoft.AspNetCore.Http.Abstractions", entity.ResultType);

			var model = await databaseOperationRepository.ToModelAsync(entity);

			Assert.IsNotNull(model);
			Assert.IsTrue(model.Result is ProblemDetails);
			Assert.AreEqual("Test", ((ProblemDetails)model.Result).Title);
			Assert.AreEqual("Error", ((ProblemDetails)model.Result).Type);
		}

		#endregion
	}
}