using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Helpers
{
	public static class DatabaseHelper
	{
		#region Fields

		public static readonly string DatabaseDirectoyPath = Path.Combine(Global.ProjectDirectoryPath, "Data");

		#endregion

		#region Methods

		public static async Task DeleteDatabasesAsync()
		{
			await DeleteSqliteDatabaseAsync();
			await DeleteSqlServerDatabaseAsync();
		}

		public static async Task DeleteSqliteDatabaseAsync(string fileName = "Database.db")
		{
			var sqliteDatabasePath = Path.Combine(DatabaseDirectoyPath, fileName);
			var sqliteContextOptionsBuilder = new DbContextOptionsBuilder();
			sqliteContextOptionsBuilder.UseSqlite($"Data Source={sqliteDatabasePath}");

			using(var context = new DbContext(sqliteContextOptionsBuilder.Options))
			{
				await context.Database.EnsureDeletedAsync();
			}
		}

		public static async Task DeleteSqlServerDatabaseAsync(string fileName = "Database.mdf")
		{
			var sqlServerDatabasePath = Path.Combine(DatabaseDirectoyPath, fileName);
			var sqlServerContextOptionsBuilder = new DbContextOptionsBuilder();
			sqlServerContextOptionsBuilder.UseSqlServer($"AttachDbFilename={sqlServerDatabasePath};Database={sqlServerDatabasePath};Integrated Security=True;Server=(LocalDb)\\MSSQLLocalDB");
			using(var context = new DbContext(sqlServerContextOptionsBuilder.Options))
			{
				await context.Database.EnsureDeletedAsync();
			}
		}

		#endregion
	}
}