using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service.Models.Data
{
	public interface IOperationContext : IDisposable
	{
		#region Properties

		DbSet<Entities.Operation> Operations { get; }

		#endregion

		#region Methods

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

		#endregion
	}
}