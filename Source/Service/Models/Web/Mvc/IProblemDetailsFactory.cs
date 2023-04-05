using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service.Models.Web.Mvc
{
	public interface IProblemDetailsFactory
	{
		#region Methods

		ProblemDetails Create(Exception exception, HttpContext httpContext = null, int? statusCode = null);

		#endregion
	}
}