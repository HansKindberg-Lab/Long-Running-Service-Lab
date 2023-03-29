namespace Service.Models.Configuration
{
	public class ExceptionHandlingOptions
	{
		#region Properties

		public virtual bool Detailed { get; set; }
		public virtual bool DeveloperExceptionPage { get; set; }
		public virtual string Path { get; set; } = "/Error";

		#endregion
	}
}