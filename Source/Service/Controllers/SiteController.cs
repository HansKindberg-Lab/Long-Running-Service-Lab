using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Models.Web.Mvc;

namespace Service.Controllers
{
	public abstract class SiteController : ControllerBase
	{
		#region Constructors

		protected SiteController(ILoggerFactory loggerFactory, IProblemDetailsFactory problemDetailsFactory)
		{
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.ExtendedProblemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
		}

		#endregion

		#region Properties

		protected internal virtual IProblemDetailsFactory ExtendedProblemDetailsFactory { get; }
		protected internal virtual ILogger Logger { get; }

		#endregion
	}
}