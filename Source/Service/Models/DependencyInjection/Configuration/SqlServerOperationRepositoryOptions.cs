using System;
using System.IO;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Service.Models.Data.SqlServer;

namespace Service.Models.DependencyInjection.Configuration
{
	public class SqlServerOperationRepositoryOptions : DatabaseOperationRepositoryOptions<SqlServerOperationContext>
	{
		#region Methods

		protected internal virtual string ResolveConnectionString(string connectionString, IHostEnvironment hostEnvironment)
		{
			if(hostEnvironment == null)
				throw new ArgumentNullException(nameof(hostEnvironment));

			if(connectionString != null)
			{
				var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

				if(!string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.AttachDBFilename))
				{
					const string dataDirectoryKey = "DataDirectory";
					var dataDirectorySubstitution = $"|{dataDirectoryKey}|";

					if(sqlConnectionStringBuilder.AttachDBFilename.StartsWith(dataDirectorySubstitution, StringComparison.OrdinalIgnoreCase))
					{
						if(AppDomain.CurrentDomain.GetData(dataDirectoryKey) is not string dataDirectoryPath)
							throw new InvalidOperationException($"DataDirectory-substitution, \"{dataDirectorySubstitution}\", in connection-string but the path is not set in AppDomain (AppDomain.CurrentDomain.SetData(\"DataDirectory\", \"[Path]\")).");

						var databaseFilePath = sqlConnectionStringBuilder.AttachDBFilename.Substring(dataDirectorySubstitution.Length);

						databaseFilePath = Path.Combine(dataDirectoryPath, databaseFilePath);

						sqlConnectionStringBuilder.AttachDBFilename = databaseFilePath;
					}
					else
					{
						if(!Path.IsPathRooted(sqlConnectionStringBuilder.AttachDBFilename))
							sqlConnectionStringBuilder.AttachDBFilename = Path.Combine(hostEnvironment.ContentRootPath, sqlConnectionStringBuilder.AttachDBFilename);
					}

					if(string.IsNullOrEmpty(sqlConnectionStringBuilder.InitialCatalog))
						sqlConnectionStringBuilder.InitialCatalog = sqlConnectionStringBuilder.AttachDBFilename;

					connectionString = sqlConnectionStringBuilder.ConnectionString;
				}
			}

			return connectionString;
		}

		protected internal override void SetOptions(IConfiguration configuration, string connectionString, IHostEnvironment hostEnvironment, DbContextOptionsBuilder optionsBuilder)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if(connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			if(hostEnvironment == null)
				throw new ArgumentNullException(nameof(hostEnvironment));

			if(optionsBuilder == null)
				throw new ArgumentNullException(nameof(optionsBuilder));

			connectionString = this.ResolveConnectionString(connectionString, hostEnvironment);

			optionsBuilder.UseSqlServer(connectionString);
		}

		#endregion
	}
}