using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Service.Models.Configuration;

namespace Service.Models.Web.Mvc
{
	public class ProblemDetailsFactory : IProblemDetailsFactory
	{
		#region Constructors

		public ProblemDetailsFactory(Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory internalFactory, IOptionsMonitor<ExceptionHandlingOptions> optionsMonitor)
		{
			this.InternalFactory = internalFactory ?? throw new ArgumentNullException(nameof(internalFactory));
			this.OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
		}

		#endregion

		#region Properties

		protected internal virtual Microsoft.AspNetCore.Mvc.Infrastructure.ProblemDetailsFactory InternalFactory { get; }
		protected internal virtual IOptionsMonitor<ExceptionHandlingOptions> OptionsMonitor { get; }

		#endregion

		#region Methods

		public virtual ProblemDetails Create(Exception exception, HttpContext httpContext = null, int? statusCode = null)
		{
			var options = this.OptionsMonitor.CurrentValue;

			var detail = options.Detailed ? exception?.ToString() : null;

			var title = exception is ServiceException ? exception.Message : exception?.GetType().Name;

			return this.InternalFactory.CreateProblemDetails(httpContext, statusCode ?? 500, title, "Error", detail);
		}

		#endregion
	}
}