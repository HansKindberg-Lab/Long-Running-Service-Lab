using System;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Models.Configuration;
using Service.Models.Logging.Extensions;
using Service.Models.Security.Configuration;

namespace Service.Models.Security
{
	public class AuthenticationMiddleware
	{
		#region Constructors

		public AuthenticationMiddleware(IOptionsMonitor<AuthenticationOptions> authenticationOptionsMonitor, IOptionsMonitor<ExceptionHandlingOptions> exceptionHandlingOptionsMonitor, IOptionsMonitor<JsonSerializerOptions> jsonOptionsMonitor, ILoggerFactory loggerFactory, RequestDelegate next)
		{
			this.AuthenticationOptionsMonitor = authenticationOptionsMonitor ?? throw new ArgumentNullException(nameof(authenticationOptionsMonitor));
			this.ExceptionHandlingOptionsMonitor = exceptionHandlingOptionsMonitor ?? throw new ArgumentNullException(nameof(exceptionHandlingOptionsMonitor));
			this.JsonOptionsMonitor = jsonOptionsMonitor ?? throw new ArgumentNullException(nameof(jsonOptionsMonitor));
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.Next = next ?? throw new ArgumentNullException(nameof(next));
		}

		#endregion

		#region Properties

		protected internal virtual IOptionsMonitor<AuthenticationOptions> AuthenticationOptionsMonitor { get; }
		protected internal virtual IOptionsMonitor<ExceptionHandlingOptions> ExceptionHandlingOptionsMonitor { get; }
		protected internal virtual IOptionsMonitor<JsonSerializerOptions> JsonOptionsMonitor { get; }
		protected internal virtual ILogger Logger { get; }
		protected internal virtual RequestDelegate Next { get; }

		#endregion

		#region Methods

		public virtual async Task Invoke(HttpContext httpContext)
		{
			if(httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			var options = this.AuthenticationOptionsMonitor.CurrentValue;

			if(!await this.ValidateHttpsAsync(httpContext, options))
			{
				await this.SetInvalidHttpsResponseAsync(httpContext);

				return;
			}

			if(!await this.ValidateIpAsync(httpContext, options))
			{
				await this.SetInvalidIpResponseAsync(httpContext);

				return;
			}

			if(!await this.ValidateKeyAsync(httpContext, options))
			{
				await this.SetInvalidKeyResponseAsync(httpContext);

				return;
			}

			await this.Next(httpContext);
		}

		protected internal virtual async Task SetInvalidHttpsResponseAsync(HttpContext httpContext)
		{
			await this.SetUnauthorizedResponseAsync(httpContext, "Not https.");
		}

		protected internal virtual async Task SetInvalidIpResponseAsync(HttpContext httpContext)
		{
			await this.SetUnauthorizedResponseAsync(httpContext, "Invalid ip.");
		}

		protected internal virtual async Task SetInvalidKeyResponseAsync(HttpContext httpContext)
		{
			await this.SetUnauthorizedResponseAsync(httpContext, "Invalid authentication-key.");
		}

		protected internal virtual async Task SetUnauthorizedResponseAsync(HttpContext httpContext, string message)
		{
			if(httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			var exceptionHandlingOptions = this.ExceptionHandlingOptionsMonitor.CurrentValue;

			const HttpStatusCode httpStatusCode = HttpStatusCode.Unauthorized;

			httpContext.Response.ContentType = MediaTypeNames.Application.Json;
			httpContext.Response.StatusCode = (int)httpStatusCode;

			var problemDetails = new ProblemDetails
			{
				Status = httpContext.Response.StatusCode,
				Title = exceptionHandlingOptions.Detailed ? message : httpStatusCode.ToString(),
				Type = "Error"
			};

			var json = JsonSerializer.Serialize(problemDetails, this.JsonOptionsMonitor.CurrentValue);

			await httpContext.Response.WriteAsync(json);
		}

		protected internal virtual async Task<bool> ValidateHttpsAsync(HttpContext httpContext, AuthenticationOptions options)
		{
			if(httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			if(options == null)
				throw new ArgumentNullException(nameof(options));

			// ReSharper disable InvertIf
			if(options.Mode.HasFlag(AuthenticationMode.Https))
			{
				this.Logger.LogInformationIfEnabled("Validating https...");

				if(!httpContext.Request.IsHttps)
				{
					this.Logger.LogWarningIfEnabled("The request is not https.");

					return false;
				}
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(true);
		}

		protected internal virtual async Task<bool> ValidateIpAsync(HttpContext httpContext, AuthenticationOptions options)
		{
			if(httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			if(options == null)
				throw new ArgumentNullException(nameof(options));

			// ReSharper disable InvertIf
			if(options.Mode.HasFlag(AuthenticationMode.Ip))
			{
				this.Logger.LogInformationIfEnabled("Validating ip...");

				var ip = httpContext.Connection.RemoteIpAddress;

				if(ip == null)
				{
					this.Logger.LogWarningIfEnabled("The remote ip-address is null.");

					return false;
				}

				if(!options.IpAddresses.Contains(ip) && !options.IpNetworks.Any(ipNetwork => ipNetwork.Contains(ip)))
				{
					this.Logger.LogWarningIfEnabled(() => $"The remote ip-address \"{ip}\" is invalid.");

					return false;
				}
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(true);
		}

		protected internal virtual async Task<bool> ValidateKeyAsync(HttpContext httpContext, AuthenticationOptions options)
		{
			if(httpContext == null)
				throw new ArgumentNullException(nameof(httpContext));

			if(options == null)
				throw new ArgumentNullException(nameof(options));

			// ReSharper disable InvertIf
			if(options.Mode.HasFlag(AuthenticationMode.Key))
			{
				this.Logger.LogInformationIfEnabled("Validating authentication-key...");

				if(!options.Keys.Any())
				{
					this.Logger.LogWarningIfEnabled("No authentication-keys configured.");

					return false;
				}

				if(!httpContext.Request.Headers.TryGetValue(options.HeaderName, out var value))
				{
					this.Logger.LogWarningIfEnabled("The request-headers does not contain an authentication-key.");

					return false;
				}

				if(!options.Keys.Contains(value))
				{
					this.Logger.LogWarningIfEnabled(() => $"The authentication-key \"{value}\" is invalid.");

					return false;
				}
			}
			// ReSharper restore InvertIf

			return await Task.FromResult(true);
		}

		#endregion
	}
}