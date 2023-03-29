namespace Client.Models.Configuration
{
	public class ServiceConnectionOptions
	{
		#region Properties

		public virtual string AuthenticationKey { get; set; }
		public virtual string OperationPathFormat { get; set; } = "/Operation/{0}";
		public virtual string OperationsPath { get; set; } = "/Operations";
		public virtual Uri Origin { get; set; }
		public virtual string ProcessPath { get; set; } = "/Process";
		public virtual string ProcessWithResultPath { get; set; } = "/ProcessWithResult";
		public virtual TimeSpan? Timeout { get; set; }

		#endregion
	}
}