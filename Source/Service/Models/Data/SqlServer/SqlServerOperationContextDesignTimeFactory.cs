using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Service.Models.Data.SqlServer
{
	public class SqlServerOperationContextDesignTimeFactory : IDesignTimeDbContextFactory<SqlServerOperationContext>
	{
		#region Methods

		public SqlServerOperationContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<SqlServerOperationContext>();
			optionsBuilder.UseSqlServer("A value that can not be empty just to be able to create/update migrations.");

			return new SqlServerOperationContext(optionsBuilder.Options);
		}

		#endregion
	}
}