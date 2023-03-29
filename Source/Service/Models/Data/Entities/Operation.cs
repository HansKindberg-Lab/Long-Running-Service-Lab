using System;

namespace Service.Models.Data.Entities
{
	public class Operation
	{
		#region Properties

		public virtual DateTimeOffset? End { get; set; }
		public virtual Guid Id { get; set; }
		public virtual string Result { get; set; }
		public virtual string ResultType { get; set; }
		public virtual DateTimeOffset Start { get; set; }

		#endregion
	}
}