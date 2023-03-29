using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service.Models.DependencyInjection.Configuration
{
	public abstract class OperationRepositoryOptions
	{
		#region Methods

		public virtual void Add(IConfiguration configuration, IHostEnvironment hostEnvironment, IServiceCollection services)
		{
			try
			{
				if(configuration == null)
					throw new ArgumentNullException(nameof(configuration));

				if(hostEnvironment == null)
					throw new ArgumentNullException(nameof(hostEnvironment));

				if(services == null)
					throw new ArgumentNullException(nameof(services));

				this.AddInternal(configuration, hostEnvironment, services);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not add operation-repository with options of type \"{this.GetType()}\".", exception);
			}
		}

		protected internal abstract void AddInternal(IConfiguration configuration, IHostEnvironment hostEnvironment, IServiceCollection services);

		public virtual void Use(IApplicationBuilder builder)
		{
			try
			{
				if(builder == null)
					throw new ArgumentNullException(nameof(builder));

				this.UseInternal(builder);
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException($"Could not use operation-repository with options of type \"{this.GetType()}\".", exception);
			}
		}

		protected internal virtual void UseInternal(IApplicationBuilder builder) { }

		#endregion
	}
}