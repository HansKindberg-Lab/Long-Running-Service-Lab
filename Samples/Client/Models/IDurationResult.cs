namespace Client.Models
{
	public interface IDurationResult
	{
		#region Properties

		TimeSpan Duration { get; }
		TimeSpan? DurationParameter { get; }
		DateTimeOffset End { get; }
		DateTimeOffset Start { get; }

		#endregion
	}
}