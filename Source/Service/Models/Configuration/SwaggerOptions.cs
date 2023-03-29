using System;
using System.Reflection;

namespace Service.Models.Configuration
{
	public class SwaggerOptions
	{
		#region Properties

		public virtual string Name { get; set; } = typeof(SwaggerOptions).Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description.Trim('.');
		public virtual string UrlSegment { get; set; } = "swagger";
		public virtual Version Version { get; set; } = new(1, 0);
		public virtual string VersionName => $"v{this.Version}";

		#endregion
	}
}