using System;
using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Models.Builder.Extensions;
using Service.Models.Data;
using Service.Models.Data.Entities;

namespace IntegrationTests.Models.Builder.Extensions
{
	[TestClass]
	public class ApplicationBuilderExtensionTest
	{
		#region Fields

		private static readonly Guid _defaultId = Guid.NewGuid();
		private static readonly DateTimeOffset _defaultStart = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

		#endregion

		#region Methods

		[ClassInitialize]
		public static async Task InitializeAsync(TestContext _)
		{
			await DatabaseHelper.DeleteDatabasesAsync();
		}

		protected internal virtual async Task Use_ShouldCreateDatabase(IConfiguration configuration)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			var services = Global.CreateServices(configuration);

			using(var serviceProvider = services.BuildServiceProvider())
			{
				var applicationBuilder = new ApplicationBuilder(serviceProvider);
				applicationBuilder.UseOperationRepository();

				var operationContextFactory = serviceProvider.GetRequiredService<IOperationContextFactory>();

				using(var operationContext = operationContextFactory.Create())
				{
					Assert.IsFalse(operationContext.Operations.Any());
					operationContext.Operations.Add(new Operation { Id = _defaultId, Start = _defaultStart });
					await operationContext.SaveChangesAsync();
				}

				using(var operationContext = operationContextFactory.Create())
				{
					Assert.AreEqual(1, operationContext.Operations.Count());
					var operation = operationContext.Operations.First();
					Assert.AreEqual(_defaultId, operation.Id);
					Assert.AreEqual(_defaultStart, operation.Start);
				}

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		[TestMethod]
		public async Task UseOperationRepository_IfSqliteOperationRepositoryConfigured_ShouldCreateDatabase()
		{
			await this.Use_ShouldCreateDatabase(Global.Configuration);
		}

		[TestMethod]
		public async Task UseOperationRepository_IfSqlServerOperationRepositoryConfigured_ShouldCreateDatabase()
		{
			await this.Use_ShouldCreateDatabase(Global.CreateConfiguration("appsettings.json", "appsettings.SqlServer.json"));
		}

		#endregion
	}
}