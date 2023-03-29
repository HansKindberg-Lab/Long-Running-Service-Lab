using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Models;
using Service.Models.Configuration;
using Service.Models.Logging.Extensions;

namespace Service.Controllers
{
	[ApiController]
	public class ServiceController : SiteController
	{
		#region Constructors

		public ServiceController(IGuidFactory guidFactory, ILoggerFactory loggerFactory, IOperationRepository operationRepository, IOptionsMonitor<ServiceOptions> optionsMonitor, ISystemClock systemClock) : base(loggerFactory)
		{
			this.GuidFactory = guidFactory ?? throw new ArgumentNullException(nameof(guidFactory));
			this.OperationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
			this.OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
			this.SystemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		}

		#endregion

		#region Properties

		protected internal virtual IGuidFactory GuidFactory { get; }
		protected internal virtual IOperationRepository OperationRepository { get; }
		protected internal virtual IOptionsMonitor<ServiceOptions> OptionsMonitor { get; }
		protected internal virtual ISystemClock SystemClock { get; }

		#endregion

		#region Methods

		[HttpGet]
		[Route("Operation/{id}")]
		public virtual async Task<IOperation> Operation(Guid id)
		{
			var operation = await this.OperationRepository.GetAsync(id);

			if(operation == null)
			{
				this.Logger.LogWarningIfEnabled($"Operation with id \"{id}\" does not exist.");

				this.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
			else
			{
				var options = this.OptionsMonitor.CurrentValue;

				if(operation.End == null)
				{
					this.HttpContext.Response.Headers.RetryAfter = $"{options.OperationRetryInterval}";
				}
				else if(options.DeleteOperationWhenCompleteAndRequested)
				{
					this.Logger.LogInformationIfEnabled($"Deleting operation with id \"{id}\"...");

					var deleted = await this.OperationRepository.DeleteAsync(id);

					if(deleted)
						this.Logger.LogInformationIfEnabled($"Operation with id \"{id}\" deleted.");
				}
			}

			return await Task.FromResult(operation);
		}

		/// <summary>
		/// This endpoint/method is only for this lab. Probably not something you would implement in the real world.
		/// </summary>
		[HttpGet]
		[Route("Operations")]
		public virtual async Task<IEnumerable<IOperation>> Operations()
		{
			return await this.OperationRepository.ListAsync();
		}

		[HttpPost]
		[Route("Process")]
		public virtual async Task<IOperation> Process(TimeSpan? duration)
		{
			var start = this.SystemClock.UtcNow;

			this.Logger.LogInformationIfEnabled("Process request starting...");

			var id = this.GuidFactory.Create();

			var operation = new Operation
			{
				Id = id,
				Start = start,
			};

			await this.OperationRepository.SaveAsync(operation);

			this.HttpContext.Response.StatusCode = (int)HttpStatusCode.Accepted;

			this.HttpContext.Response.Headers.Location = $"/{nameof(this.Operation)}/{operation.Id}";

			_ = Task.Run(async () =>
			{
				var result = await this.ProcessInternal(duration, start);

				var completedOperation = new Operation
				{
					End = result.End,
					Id = id,
					Result = result,
					Start = start
				};

				await this.OperationRepository.SaveAsync(completedOperation);
			});

			this.Logger.LogInformationIfEnabled("Process request finished.");

			return await Task.FromResult(operation);
		}

		protected internal virtual async Task<IDurationResult> ProcessInternal(TimeSpan? duration, DateTimeOffset start)
		{
			if(duration != null)
				await Task.Delay(duration.Value);

			var end = this.SystemClock.UtcNow;

			var result = new DurationResult
			{
				Duration = end - start,
				DurationParameter = duration,
				End = end,
				Start = start
			};

			return result;
		}

		[HttpPost]
		[Route("ProcessWithResult")]
		public virtual async Task<IDurationResult> ProcessWithResult(TimeSpan? duration)
		{
			var start = this.SystemClock.UtcNow;

			this.Logger.LogInformationIfEnabled("ProcessWithResult request starting...");

			var result = await this.ProcessInternal(duration, start);

			this.Logger.LogInformationIfEnabled("ProcessWithResult request finished.");

			return await Task.FromResult(result);
		}

		#endregion
	}
}