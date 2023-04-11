using System;

namespace Service.Models.Extensions
{
	public static class TypeExtension
	{
		#region Methods

		public static string QualifiedName(this Type type)
		{
			if(type == null)
				throw new ArgumentNullException(nameof(type));

			return $"{type.FullName}, {type.Assembly.GetName().Name}";
		}

		#endregion
	}
}