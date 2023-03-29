using System;

namespace Service.Models
{
	public class Operation : IOperation
	{
		#region Properties

		public virtual DateTimeOffset? End { get; set; }
		public virtual Guid Id { get; set; }
		public virtual object Result { get; set; }
		public virtual DateTimeOffset Start { get; set; }

		#endregion
	}
}