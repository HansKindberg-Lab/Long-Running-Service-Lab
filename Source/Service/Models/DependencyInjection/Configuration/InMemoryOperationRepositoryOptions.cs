using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Service.Models.DependencyInjection.Configuration
{
	public class InMemoryOperationRepositoryOptions : OperationRepositoryOptions
	{
		#region Methods

		protected internal override void AddInternal(IConfiguration configuration, IHostEnvironment hostEnvironment, IServiceCollection services)
		{
			if(services == null)
				throw new ArgumentNullException(nameof(services));

			services.AddSingleton<IOperationRepository, InMemoryOperationRepository>();
		}

		#endregion
	}
}