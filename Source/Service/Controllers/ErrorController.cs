using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Models.Logging.Extensions;
using Service.Models.Web.Mvc;

namespace Service.Controllers
{
	public class ErrorController : SiteController
	{
		#region Constructors

		public ErrorController(ILoggerFactory loggerFactory, IProblemDetailsFactory problemDetailsFactory) : base(loggerFactory, problemDetailsFactory) { }

		#endregion

		#region Methods

		[ApiExplorerSettings(IgnoreApi = true)]
		[Route("Error")]
		public virtual async Task<IActionResult> Index()
		{
			var exception = this.HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

			this.Logger.LogErrorIfEnabled(exception, "Error");

			var problemDetails = this.ExtendedProblemDetailsFactory.Create(exception, this.HttpContext);

			return await Task.FromResult(new ObjectResult(problemDetails)
			{
				StatusCode = problemDetails.Status
			}).ConfigureAwait(false);
		}

		#endregion
	}
}