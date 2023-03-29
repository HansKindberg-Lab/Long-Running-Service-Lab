using Microsoft.EntityFrameworkCore;

namespace Service.Models.Data.Sqlite
{
	public class SqliteOperationContext : OperationContext<SqliteOperationContext>
	{
		#region Constructors

		public SqliteOperationContext(DbContextOptions<SqliteOperationContext> options) : base(options) { }

		#endregion
	}
}