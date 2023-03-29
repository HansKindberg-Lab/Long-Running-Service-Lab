namespace Service.Models.Configuration
{
	public class ServiceOptions
	{
		#region Properties

		public virtual bool DeleteOperationWhenCompleteAndRequested { get; set; } = true;
		public virtual int OperationRetryInterval { get; set; } = 30;

		#endregion
	}
}