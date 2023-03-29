using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Models.DependencyInjection.Configuration;

namespace UnitTests.Models.DependencyInjection.Configuration
{
	[TestClass]
	public class SqlServerOperationRepositoryOptionsTest
	{
		#region Fields

		private const string _defaultContentRootPath = @"Q:\Directory";

		#endregion

		#region Methods

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

		[TestMethod]
		public async Task ResolveConnectionString_IfTheConnectionStringParameterContainsAttachDbFileNameAsRelativePath_ShouldWorkProperly()
		{
			await Task.CompletedTask;

			var hostEnvironment = await this.CreateHostEnvironmentAsync();

			var connectionString = "AttachDBFilename=Data/Database.mdf";
			Assert.AreEqual(@"AttachDbFilename=Q:\Directory\Data/Database.mdf;Initial Catalog=Q:\Directory\Data/Database.mdf", new SqlServerOperationRepositoryOptions().ResolveConnectionString(connectionString, hostEnvironment));

			connectionString = @"AttachDBFilename=Data\Database.mdf";
			Assert.AreEqual(@"AttachDbFilename=Q:\Directory\Data\Database.mdf;Initial Catalog=Q:\Directory\Data\Database.mdf", new SqlServerOperationRepositoryOptions().ResolveConnectionString(connectionString, hostEnvironment));
		}

		[TestMethod]
		public async Task ResolveConnectionString_IfTheConnectionStringParameterIsEmpty_ShouldReturnAnEmptyString()
		{
			await Task.CompletedTask;

			Assert.AreEqual(string.Empty, new SqlServerOperationRepositoryOptions().ResolveConnectionString(string.Empty, Mock.Of<IHostEnvironment>()));
		}

		[TestMethod]
		public async Task ResolveConnectionString_IfTheConnectionStringParameterIsNull_ShouldReturnNull()
		{
			await Task.CompletedTask;

			Assert.IsNull(new SqlServerOperationRepositoryOptions().ResolveConnectionString(null, Mock.Of<IHostEnvironment>()));
		}

		#endregion
	}
}