using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Service.Models.Data.Sqlite;

namespace Service.Models.DependencyInjection.Configuration
{
	public class SqliteOperationRepositoryOptions : DatabaseOperationRepositoryOptions<SqliteOperationContext>
	{
		#region Methods

		protected internal override void SetOptions(IConfiguration configuration, string connectionString, IHostEnvironment hostEnvironment, DbContextOptionsBuilder optionsBuilder)
		{
			if(connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			if(optionsBuilder == null)
				throw new ArgumentNullException(nameof(optionsBuilder));

			optionsBuilder.UseSqlite(connectionString);
		}

		#endregion
	}
}