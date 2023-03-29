using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Models.DependencyInjection.Configuration;

namespace IntegrationTests.Models.DependencyInjection.Configuration
{
	[TestClass]
	public class SqlServerOperationRepositoryOptionsTest
	{
		#region Fields

		private const string _dataDirectoryKey = "DataDirectory";
		private static object _dataDirectoryObject;
		private const string _defaultContentRootPath = @"Q:\Directory";

		#endregion

		#region Methods

		[ClassCleanup]
		public static async Task CleanupAsync()
		{
			await Task.CompletedTask;
		}

		[TestCleanup]
		public async Task CleanupTestAsync()
		{
			await Task.CompletedTask;

			AppDomain.CurrentDomain.SetData(_dataDirectoryKey, _dataDirectoryObject);
		}

		protected internal virtual async Task<IHostEnvironment> CreateHostEnvironmentAsync()
		{
			return await this.CreateHostEnvironmentAsync(_defaultContentRootPath);
		}

		protected internal virtual async Task<IHostEnvironment> CreateHostEnvironmentAsync(string contentRootPath)
		{
			var hostEnvironmentMock = new Mock<IHostEnvironment>();

			hostEnvironmentMock.Setup(hostEnvironment => hostEnvironment.ContentRootPath).Returns(contentRootPath);

			return await Task.FromResult(hostEnvironmentMock.Object);
		}

		[ClassInitialize]
		public static async Task InitializeAsync(TestContext _)
		{
			await Task.CompletedTask;

			_dataDirectoryObject = AppDomain.CurrentDomain.GetData(_dataDirectoryKey);
		}

		[TestInitialize]
		public async Task InitializeTestAsync()
		{
			await Task.CompletedTask;
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task ResolveConnectionString_IfTheConnectionStringParameterContainsAttachDbFileNameWithDataDirectorySubstitution_And_TheDataDirectoryIsNotSet_ShouldThrowAnException()
		{
			await Task.CompletedTask;

			Assert.IsNull(AppDomain.CurrentDomain.GetData(_dataDirectoryKey));
			//var hostEnvironment = await this.CreateHostEnvironmentAsync();
			var connectionString = "AttachDBFilename=|DataDirectory|Database.mdf";

			try
			{
				var _ = new SqlServerOperationRepositoryOptions().ResolveConnectionString(connectionString, Mock.Of<IHostEnvironment>());
			}
			catch(Exception exception)
			{
				if(exception is InvalidOperationException { Message: "DataDirectory-substitution, \"|DataDirectory|\", in connection-string but the path is not set in AppDomain (AppDomain.CurrentDomain.SetData(\"DataDirectory\", \"[Path]\"))." })
					throw;
			}
		}

		[TestMethod]
		public async Task ResolveConnectionString_IfTheConnectionStringParameterContainsAttachDbFileNameWithDataDirectorySubstitution_And_TheDataDirectoryIsSet_ShouldWorkProperly()
		{
			await Task.CompletedTask;

			AppDomain.CurrentDomain.SetData(_dataDirectoryKey, _defaultContentRootPath);
			var connectionString = "AttachDBFilename=|DataDirectory|Database.mdf";

			Assert.AreEqual(@"AttachDbFilename=Q:\Directory\Database.mdf;Initial Catalog=Q:\Directory\Database.mdf", new SqlServerOperationRepositoryOptions().ResolveConnectionString(connectionString, Mock.Of<IHostEnvironment>()));
		}

		#endregion
	}
}