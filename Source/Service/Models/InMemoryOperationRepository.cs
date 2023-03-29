using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Models
{
	public class InMemoryOperationRepository : IOperationRepository
	{
		#region Properties

		protected internal virtual ConcurrentDictionary<Guid, IOperation> Operations { get; } = new();

		#endregion

		#region Methods

		public virtual async Task<bool> DeleteAsync(Guid id)
		{
			return await Task.FromResult(this.Operations.TryRemove(id, out _));
		}

		public virtual async Task<IOperation> GetAsync(Guid id)
		{
			if(this.Operations.TryGetValue(id, out var operation))
				return await Task.FromResult(operation);

			return null;
		}

		public virtual async Task<IEnumerable<IOperation>> ListAsync()
		{
			return await Task.FromResult(this.Operations.Values);
		}

		public virtual async Task SaveAsync(IOperation operation)
		{
			if(operation == null)
				throw new ArgumentNullException(nameof(operation));

			if(operation.Id == Guid.Empty)
				throw new ArgumentException("The id can not be an empty guid.", nameof(operation));

			this.Operations.AddOrUpdate(operation.Id, operation, (_, _) => operation);

			await Task.CompletedTask;
		}

		#endregion
	}
}