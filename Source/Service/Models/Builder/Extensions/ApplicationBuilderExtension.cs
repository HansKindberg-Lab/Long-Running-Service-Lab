using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Service.Models.Configuration;
using Service.Models.Configuration.Extensions;
using Service.Models.DependencyInjection.Configuration;
using Service.Models.Security;

namespace Service.Models.Builder.Extensions
{
	public static class ApplicationBuilderExtension
	{
		#region Methods

		public static IApplicationBuilder Use(this IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			var exceptionHandling = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<ExceptionHandlingOptions>>().Value;

			if(exceptionHandling.DeveloperExceptionPage)
				applicationBuilder.UseDeveloperExceptionPage();
			else
				applicationBuilder.UseExceptionHandler(exceptionHandling.Path);

			var configuration = applicationBuilder.ApplicationServices.GetRequiredService<IConfiguration>();

			if(configuration.IsEnabledSection(ConfigurationKeys.ForwardedHeadersPath))
				applicationBuilder.UseForwardedHeaders();

			if(configuration.IsEnabledSection(ConfigurationKeys.HstsPath))
				applicationBuilder.UseHsts();

			if(configuration.IsEnabledSection(ConfigurationKeys.HttpsRedirectionPath))
				applicationBuilder.UseHttpsRedirection();

			applicationBuilder.UseOperationRepository();

			var swaggerOptions = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<SwaggerOptions>>().Value;

			applicationBuilder.UseSwagger();
			applicationBuilder.UseSwaggerUI(options => { options.SwaggerEndpoint($"/{swaggerOptions.UrlSegment}/{swaggerOptions.VersionName}/{swaggerOptions.UrlSegment}.json", swaggerOptions.Name); });
			applicationBuilder.UseRouting();
			applicationBuilder.UseMiddleware<AuthenticationMiddleware>();
			applicationBuilder.UseEndpoints(endpoints => { endpoints.MapControllers(); });

			return applicationBuilder;
		}

		public static IApplicationBuilder UseOperationRepository(this IApplicationBuilder applicationBuilder)
		{
			if(applicationBuilder == null)
				throw new ArgumentNullException(nameof(applicationBuilder));

			applicationBuilder.ApplicationServices.GetRequiredService<OperationRepositoryOptions>().Use(applicationBuilder);

			return applicationBuilder;
		}

		#endregion
	}
}