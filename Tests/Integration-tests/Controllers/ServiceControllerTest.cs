using System;
using System.Linq;
using System.Threading.Tasks;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Controllers;
using Service.Models;
using Service.Models.Configuration;
using Service.Models.Data;
using Service.Models.DependencyInjection.Configuration;
using Service.Models.Web.Mvc;

namespace IntegrationTests.Controllers
{
	[TestClass]
	public class ServiceControllerTest
	{
		#region Fields

		private static readonly Guid _id = new("89d55e4d-e996-46cc-98fe-1b90f1c786b1");
		private static readonly DateTimeOffset _utcNow = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);

		#endregion

		#region Methods

		protected internal virtual async Task<HttpContext> CreateHttpContextAsync(IServiceProvider serviceProvider)
		{
			var httpContext = new DefaultHttpContext
			{
				RequestServices = serviceProvider
			};

			return await Task.FromResult(httpContext);
		}

		protected internal virtual async Task<ServiceController> CreateServiceControllerAsync(IServiceProvider serviceProvider)
		{
			return await this.CreateServiceControllerAsync(_id, serviceProvider, _utcNow);
		}

		protected internal virtual async Task<ServiceController> CreateServiceControllerAsync(Guid id, IServiceProvider serviceProvider, DateTimeOffset utcNow)
		{
			var guidFactoryMock = new Mock<IGuidFactory>();
			guidFactoryMock.Setup(guidFactory => guidFactory.Create()).Returns(id);

			var systemClockMock = new Mock<ISystemClock>();
			systemClockMock.Setup(systemClock => systemClock.UtcNow).Returns(utcNow);

			return await this.CreateServiceControllerAsync(guidFactoryMock.Object, serviceProvider, systemClockMock.Object);
		}

		protected internal virtual async Task<ServiceController> CreateServiceControllerAsync(IGuidFactory guidFactory, IServiceProvider serviceProvider, ISystemClock systemClock)
		{
			var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
			var operationRepository = serviceProvider.GetRequiredService<IOperationRepository>();
			var problemDetailsFactory = serviceProvider.GetRequiredService<IProblemDetailsFactory>();
			var serviceOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<ServiceOptions>>();

			var serviceController = new ServiceController(guidFactory, loggerFactory, operationRepository, serviceOptionsMonitor, problemDetailsFactory, systemClock)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = await this.CreateHttpContextAsync(serviceProvider)
				}
			};

			return await Task.FromResult(serviceController);
		}

		protected internal virtual async Task<ServiceProvider> CreateServiceProviderAsync()
		{
			var serviceProvider = Global.CreateServices().BuildServiceProvider();

			var operationRepositoryOptions = serviceProvider.GetRequiredService<OperationRepositoryOptions>();

			operationRepositoryOptions.Use(new ApplicationBuilder(serviceProvider));

			return await Task.FromResult(serviceProvider);
		}

		[ClassInitialize]
		public static async Task InitializeAsync(TestContext _)
		{
			await DatabaseHelper.DeleteDatabasesAsync();
		}

		[TestMethod]
		public async Task Operation_Test()
		{
			using(var serviceProvider = await this.CreateServiceProviderAsync())
			{
				var serviceController = await this.CreateServiceControllerAsync(serviceProvider);

				var operation = await serviceController.Operation(_id);
				Assert.IsNull(operation);
				Assert.AreEqual(404, serviceController.Response.StatusCode);

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		[TestMethod]
		public async Task Operations_Test()
		{
			using(var serviceProvider = await this.CreateServiceProviderAsync())
			{
				var serviceController = await this.CreateServiceControllerAsync(serviceProvider);

				var operations = await serviceController.Operations();
				Assert.IsFalse(operations.Any());
				Assert.AreEqual(200, serviceController.Response.StatusCode);

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		[TestMethod]
		public async Task Process_Test()
		{
			using(var serviceProvider = await this.CreateServiceProviderAsync())
			{
				var serviceController = await this.CreateServiceControllerAsync(serviceProvider);

				var operation = await serviceController.Process(null);
				Assert.IsNotNull(operation);
				Assert.IsNull(operation.End);
				Assert.AreEqual(_id, operation.Id);
				Assert.IsNull(operation.Result);
				Assert.AreEqual(_utcNow, operation.Start);
				Assert.AreEqual($"/Operation/{_id}", (string)serviceController.Response.Headers.Location);
				Assert.AreEqual(202, serviceController.Response.StatusCode);

				await Task.Delay(500);

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					Assert.AreEqual(1, await operationContext.Operations.CountAsync());
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		[TestMethod]
		public async Task Process_ThrowException_Test()
		{
			using(var serviceProvider = await this.CreateServiceProviderAsync())
			{
				var serviceController = await this.CreateServiceControllerAsync(serviceProvider);

				var operation = await serviceController.Process(null, true);
				Assert.IsNotNull(operation);
				Assert.IsNull(operation.End);
				Assert.AreEqual(_id, operation.Id);
				Assert.IsNull(operation.Result);
				Assert.AreEqual(_utcNow, operation.Start);
				Assert.AreEqual($"/Operation/{_id}", (string)serviceController.Response.Headers.Location);
				Assert.AreEqual(202, serviceController.Response.StatusCode);

				await Task.Delay(500);

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					Assert.AreEqual(1, await operationContext.Operations.CountAsync());
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		[TestMethod]
		public async Task ProcessWithResult_Test()
		{
			using(var serviceProvider = await this.CreateServiceProviderAsync())
			{
				var serviceController = await this.CreateServiceControllerAsync(serviceProvider);

				var result = await serviceController.ProcessWithResult(null);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Duration >= TimeSpan.Zero);
				Assert.IsNull(result.DurationParameter);
				Assert.IsTrue(result.End >= _utcNow);
				Assert.AreEqual(_utcNow, result.Start);
				Assert.AreEqual(200, serviceController.Response.StatusCode);

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public async Task ProcessWithResult_ThrowException_ShouldThrowAnInvalidOperationException()
		{
			using(var serviceProvider = await this.CreateServiceProviderAsync())
			{
				var serviceController = await this.CreateServiceControllerAsync(serviceProvider);

				try
				{
					await serviceController.ProcessWithResult(null, true);
				}
				catch(InvalidOperationException invalidOperationException)
				{
					if(invalidOperationException.Message.Equals("Requested exception.", StringComparison.Ordinal))
						throw;
				}
				finally
				{
					using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
					{
						await operationContext.Database.EnsureDeletedAsync();
					}
				}
			}
		}

		[TestMethod]
		public async Task Scenario_Test()
		{
			var duration = TimeSpan.FromSeconds(5);
			var guidFactoryMock = new Mock<GuidFactory>();
			guidFactoryMock.Setup(guidFactory => guidFactory.Create()).Returns(_id);
			var guidFactory = guidFactoryMock.Object;
			var systemClock = new SystemClock();
			var testStart = systemClock.UtcNow;

			async Task<ServiceController> ServiceControllerFunction(IServiceProvider serviceProvider)
			{
				return await this.CreateServiceControllerAsync(guidFactory, serviceProvider, systemClock);
			}

			using(var serviceProvider = await this.CreateServiceProviderAsync())
			{
				var serviceController = await ServiceControllerFunction(serviceProvider);
				var operation = await serviceController.Process(duration);
				Assert.IsNotNull(operation);
				Assert.IsNull(operation.End);
				Assert.AreEqual(_id, operation.Id);
				Assert.IsNull(operation.Result);
				Assert.IsTrue(operation.Start >= testStart);
				Assert.AreEqual(1, serviceController.Response.Headers.Count);
				Assert.AreEqual($"/Operation/{_id}", (string)serviceController.Response.Headers.Location);
				Assert.AreEqual(202, serviceController.Response.StatusCode);

				var start = operation.Start;

				await Task.Delay(2000);

				serviceController = await ServiceControllerFunction(serviceProvider);
				operation = await serviceController.Operation(_id);
				Assert.IsNotNull(operation);
				Assert.IsNull(operation.End);
				Assert.AreEqual(_id, operation.Id);
				Assert.IsNull(operation.Result);
				Assert.AreEqual(start, operation.Start);
				Assert.AreEqual(1, serviceController.Response.Headers.Count);
				Assert.AreEqual("30", (string)serviceController.Response.Headers.RetryAfter);
				Assert.AreEqual(200, serviceController.Response.StatusCode);

				await Task.Delay(2000);

				serviceController = await ServiceControllerFunction(serviceProvider);
				operation = await serviceController.Operation(_id);
				Assert.IsNotNull(operation);
				Assert.IsNull(operation.End);
				Assert.AreEqual(_id, operation.Id);
				Assert.IsNull(operation.Result);
				Assert.AreEqual(start, operation.Start);
				Assert.AreEqual(1, serviceController.Response.Headers.Count);
				Assert.AreEqual("30", (string)serviceController.Response.Headers.RetryAfter);
				Assert.AreEqual(200, serviceController.Response.StatusCode);

				await Task.Delay(2000);

				serviceController = await ServiceControllerFunction(serviceProvider);
				operation = await serviceController.Operation(_id);
				Assert.IsNotNull(operation);
				Assert.IsNotNull(operation.End);
				Assert.IsTrue(operation.End >= start.Add(duration));
				Assert.AreEqual(_id, operation.Id);
				Assert.IsNotNull(operation.Result);
				var result = operation.Result as IDurationResult;
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Duration >= duration);
				Assert.AreEqual(result.DurationParameter, duration);
				Assert.IsTrue(result.End >= start.Add(duration));
				Assert.AreEqual(start, result.Start);
				Assert.AreEqual(start, operation.Start);
				Assert.AreEqual(0, serviceController.Response.Headers.Count);
				Assert.IsFalse(serviceController.Response.Headers.RetryAfter.Any());
				Assert.AreEqual(200, serviceController.Response.StatusCode);

				await Task.Delay(500);

				using(var operationContext = serviceProvider.GetRequiredService<OperationContext>())
				{
					await operationContext.Database.EnsureDeletedAsync();
				}
			}
		}

		#endregion
	}
}