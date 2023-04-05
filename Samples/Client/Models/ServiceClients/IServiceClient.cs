namespace Client.Models.ServiceClients
{
	public interface IServiceClient
	{
		#region Methods

		Task<IOperation> Operation(Guid? id);
		Task<IEnumerable<IOperation>> Operations();
		Task<IOperation> Process(TimeSpan? duration, bool throwException);
		Task<IDurationResult> ProcessWithResult(TimeSpan? duration, bool throwException);

		#endregion
	}
}