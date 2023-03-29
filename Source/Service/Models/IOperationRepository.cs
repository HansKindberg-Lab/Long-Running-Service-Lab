using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Models
{
	public interface IOperationRepository
	{
		#region Methods

		Task<bool> DeleteAsync(Guid id);
		Task<IOperation> GetAsync(Guid id);
		Task<IEnumerable<IOperation>> ListAsync();
		Task SaveAsync(IOperation operation);

		#endregion
	}
}