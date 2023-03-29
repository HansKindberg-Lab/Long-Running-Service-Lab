using System.Net;

namespace Client.Models
{
	public interface IOperation
	{
		#region Properties

		DateTimeOffset? End { get; }
		HttpStatusCode? HttpStatusCode { get; }
		Guid Id { get; }
		string Location { get; }

		/// <summary>
		/// The local path at the client.
		/// </summary>
		string Path { get; }

		object Result { get; }
		DateTimeOffset? RetryAfterDate { get; }
		double? RetryAfterSeconds { get; }
		DateTimeOffset Start { get; }

		#endregion
	}
}