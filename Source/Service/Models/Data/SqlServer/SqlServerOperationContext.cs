using Microsoft.EntityFrameworkCore;

namespace Service.Models.Data.SqlServer
{
	public class SqlServerOperationContext : OperationContext<SqlServerOperationContext>
	{
		#region Constructors

		public SqlServerOperationContext(DbContextOptions<SqlServerOperationContext> options) : base(options) { }

		#endregion
	}
}