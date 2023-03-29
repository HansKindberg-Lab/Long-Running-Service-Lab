using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Models;
using Service.Models.Configuration;
using Service.Models.Logging.Extensions;

namespace Service.Controllers
{
	public class ErrorController : SiteController
	{
		#region Constructors

		public ErrorController(ILoggerFactory loggerFactory, IOptions<ExceptionHandlingOptions> options) : base(loggerFactory)
		{
			this.Options = options ?? throw new ArgumentNullException(nameof(options));
		}

		#endregion

		#region Properties

		protected internal virtual IOptions<ExceptionHandlingOptions> Options { get; }

		#endregion

		#region Methods

		protected internal virtual ProblemDetails CreateProblemDetails(Exception exception)
		{
			var details = this.Options.Value.Detailed ? exception?.ToString() : null;

			var title = exception is ServiceException ? exception.Message : exception?.GetType().Name;

			return this.ProblemDetailsFactory.CreateProblemDetails(this.HttpContext, 500, title, "Error", details);
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[Route("Error")]
		public virtual async Task<IActionResult> Index()
		{
			var exception = this.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

			this.Logger.LogErrorIfEnabled(exception, "Error");

			var problemDetails = this.CreateProblemDetails(exception);

			return await Task.FromResult(new ObjectResult(problemDetails)
			{
				StatusCode = problemDetails.Status
			}).ConfigureAwait(false);
		}

		#endregion
	}
}