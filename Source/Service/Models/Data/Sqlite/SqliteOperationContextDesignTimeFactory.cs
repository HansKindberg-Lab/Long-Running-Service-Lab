using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Service.Models.Data.Sqlite
{
	public class SqliteOperationContextDesignTimeFactory : IDesignTimeDbContextFactory<SqliteOperationContext>
	{
		#region Methods

		public SqliteOperationContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SqliteOperationContext>();
			optionsBuilder.UseSqlite("A value that can not be empty just to be able to create/update migrations.");

			return new SqliteOperationContext(optionsBuilder.Options);
		}

		#endregion
	}
}