using System.Text.Json;
using Client.Models.Json.Extensions;

namespace Client.Models.Extensions
{
	public static class ObjectExtension
	{
		#region Fields

		private static readonly JsonSerializerOptions _jsonSerializerOptions = CreateJsonSerializerOptions();

		#endregion

		#region Methods

		private static JsonSerializerOptions CreateJsonSerializerOptions()
		{
			var jsonSerializerOptions = new JsonSerializerOptions();
			jsonSerializerOptions.SetDefaults();
			return jsonSerializerOptions;
		}

		public static string ToJson(this object instance)
		{
			return JsonSerializer.Serialize(instance, _jsonSerializerOptions);
		}

		#endregion
	}
}