using System;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Service.Models.Configuration;
using Service.Models.DependencyInjection.Configuration;
using Service.Models.Json.Extensions;
using Service.Models.Security;
using Service.Models.Security.Configuration;
using Service.Models.Web.Mvc;

namespace Service.Models.DependencyInjection.Extensions
{
	public static class ServiceCollectionExtension
	{
		#region Methods

		public static IServiceCollection Add(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if(hostEnvironment == null)
				throw new ArgumentNullException(nameof(hostEnvironment));

			var authenticationSection = configuration.GetSection(ConfigurationKeys.AuthenticationPath);
			services.Configure<AuthenticationOptions>(authenticationSection);
			services.Configure<ExceptionHandlingOptions>(configuration.GetSection(ConfigurationKeys.ExceptionHandlingPath));
			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.AllowedHosts.Clear();
				options.KnownNetworks.Clear();
				options.KnownProxies.Clear();
			});
			services.Configure<ForwardedHeadersOptions>(configuration.GetSection(ConfigurationKeys.ForwardedHeadersPath));
			services.Configure<HstsOptions>(configuration.GetSection(ConfigurationKeys.HstsPath));
			services.Configure<HttpsRedirectionOptions>(configuration.GetSection(ConfigurationKeys.HttpsRedirectionPath));
			var jsonSection = configuration.GetSection(ConfigurationKeys.JsonPath);
			services.Configure<JsonSerializerOptions>(options =>
			{
				options.SetDefaults();
				jsonSection.Bind(options);
			});
			services.Configure<ServiceOptions>(configuration.GetSection(ConfigurationKeys.ServicePath));
			var swaggerSection = configuration.GetSection(ConfigurationKeys.SwaggerPath);
			services.Configure<SwaggerOptions>(swaggerSection);

			services.AddOperationRepository(configuration, hostEnvironment);

			services.AddSingleton<IGuidFactory, GuidFactory>();
			services.AddSingleton<IProblemDetailsFactory, ProblemDetailsFactory>();
			services.AddSingleton<ISystemClock, SystemClock>();

			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.SetDefaults();
				jsonSection.Bind(options.JsonSerializerOptions);
			});

			var authenticationOptions = new AuthenticationOptions();
			authenticationSection.Bind(authenticationOptions);
			var swaggerOptions = new SwaggerOptions();
			swaggerSection.Bind(swaggerOptions);

			services.AddSwaggerGen(options =>
			{
				if(authenticationOptions.Mode.HasFlag(AuthenticationMode.Key))
				{
					const string name = "Authentication-key";

					options.AddSecurityDefinition(name, new OpenApiSecurityScheme
					{
						Name = authenticationOptions.HeaderName,
						In = ParameterLocation.Header,
						Type = SecuritySchemeType.ApiKey
					});

					options.AddSecurityRequirement(new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme
							{
								Reference = new OpenApiReference
								{
									Id = name,
									Type = ReferenceType.SecurityScheme
								}
							},
							Array.Empty<string>()
						}
					});
				}

				// Temporary fix: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/2505#issuecomment-1279557743
				options.MapType<TimeSpan>(() => new OpenApiSchema
				{
					Example = new OpenApiString("00:00:00"),
					Type = "string"
				});

				options.SwaggerDoc(swaggerOptions.VersionName, new OpenApiInfo { Title = swaggerOptions.Name, Version = swaggerOptions.VersionName });
			});

			return services;
		}

		public static IServiceCollection AddOperationRepository(this IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if(hostEnvironment == null)
				throw new ArgumentNullException(nameof(hostEnvironment));

			var typeValue = configuration.GetValue<string>($"{ConfigurationKeys.OperationRepositoryConfigurationPath}:Type");

			var type = typeValue != null ? Type.GetType(typeValue, true, true) : typeof(InMemoryOperationRepositoryOptions);

			var operationRepositoryOptions = (OperationRepositoryOptions)Activator.CreateInstance(type);

			configuration.GetSection(ConfigurationKeys.OperationRepositoryConfigurationPath).Bind(operationRepositoryOptions);

			operationRepositoryOptions.Add(configuration, hostEnvironment, services);

			services.AddSingleton(operationRepositoryOptions);

			return services;
		}

		#endregion
	}
}