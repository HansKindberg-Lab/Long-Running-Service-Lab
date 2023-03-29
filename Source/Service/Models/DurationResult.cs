using System;

namespace Service.Models
{
	public class DurationResult : IDurationResult
	{
		#region Properties

		public virtual TimeSpan Duration { get; set; }
		public virtual TimeSpan? DurationParameter { get; set; }
		public virtual DateTimeOffset End { get; set; }
		public virtual DateTimeOffset Start { get; set; }

		#endregion
	}
}