using System.Net;

namespace Client.Models
{
	public class Operation : IOperation
	{
		#region Properties

		public virtual DateTimeOffset? End { get; set; }
		public virtual HttpStatusCode? HttpStatusCode { get; set; }
		public virtual Guid Id { get; set; }
		public virtual string Location { get; set; }
		public virtual string Path { get; set; }
		public virtual object Result { get; set; }
		public virtual DateTimeOffset? RetryAfterDate { get; set; }
		public virtual double? RetryAfterSeconds { get; set; }
		public virtual DateTimeOffset Start { get; set; }

		#endregion
	}
}