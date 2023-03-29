using System;
using Microsoft.Extensions.Configuration;

namespace Service.Models.Configuration.Extensions
{
	public static class ConfigurationExtension
	{
		#region Methods

		public static bool IsEnabledSection(this IConfiguration configuration, string key)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			return configuration.GetValue($"{key}:Enabled", false);
		}

		#endregion
	}
}