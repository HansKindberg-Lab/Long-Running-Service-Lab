namespace Client.Models.Configuration
{
	public interface IServiceConnectionOptionsParser
	{
		#region Methods

		ServiceConnectionOptions Parse(string value);

		#endregion
	}
}