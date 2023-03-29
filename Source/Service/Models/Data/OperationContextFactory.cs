using System;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Models.Data
{
	public class OperationContextFactory : IOperationContextFactory
	{
		#region Constructors

		public OperationContextFactory(IServiceProvider serviceProvider)
		{
			this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		#endregion

		#region Properties

		protected internal virtual IServiceProvider ServiceProvider { get; }

		#endregion

		#region Methods

		public virtual IOperationContext Create()
		{
			return this.ServiceProvider.GetRequiredService<IOperationContext>();
		}

		#endregion
	}
}