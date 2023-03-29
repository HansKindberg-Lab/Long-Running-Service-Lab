namespace Service.Models.Data
{
	public interface IOperationContextFactory
	{
		#region Methods

		IOperationContext Create();

		#endregion
	}
}