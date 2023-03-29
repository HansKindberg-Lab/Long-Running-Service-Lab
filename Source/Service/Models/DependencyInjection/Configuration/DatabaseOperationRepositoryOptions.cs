using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Models.Data;

namespace Service.Models.DependencyInjection.Configuration
{
	public abstract class DatabaseOperationRepositoryOptions<T> : OperationRepositoryOptions where T : OperationContext
	{
		#region Properties

		public virtual string ConnectionStringName { get; set; } = "Operation";

		#endregion

		#region Methods

		protected internal override void AddInternal(IConfiguration configuration, IHostEnvironment hostEnvironment, IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			var connectionString = this.GetConnectionString(configuration);

			services.AddDbContext<T>(optionsBuilder => this.SetOptions(configuration, connectionString, hostEnvironment, optionsBuilder), ServiceLifetime.Transient, ServiceLifetime.Transient);
			services.AddSingleton<IOperationContextFactory, OperationContextFactory>();
			services.AddSingleton<IOperationRepository, DatabaseOperationRepository>();
			services.AddTransient<IOperationContext>(serviceProvider => serviceProvider.GetRequiredService<OperationContext>());
			services.AddTransient<OperationContext>(serviceProvider => serviceProvider.GetRequiredService<T>());
		}

		protected internal virtual string GetConnectionString(IConfiguration configuration)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			return configuration.GetConnectionString(this.ConnectionStringName) ?? throw new InvalidOperationException($"Could not find a connection-string with name \"{this.ConnectionStringName}\".");
		}

		protected internal abstract void SetOptions(IConfiguration configuration, string connectionString, IHostEnvironment hostEnvironment, DbContextOptionsBuilder optionsBuilder);

		protected internal override void UseInternal(IApplicationBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			// ReSharper disable ConvertToUsingDeclaration
			using(var operationContext = builder.ApplicationServices.GetRequiredService<OperationContext>())
			{
				operationContext.Database.Migrate();
			}
			// ReSharper restore ConvertToUsingDeclaration
		}

		#endregion
	}
}