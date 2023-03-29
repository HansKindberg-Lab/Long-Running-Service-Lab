using System;

namespace Service.Models
{
	public interface IOperation
	{
		#region Properties

		DateTimeOffset? End { get; }
		Guid Id { get; }
		object Result { get; }
		DateTimeOffset Start { get; }

		#endregion
	}
}