namespace Client.Models.Configuration
{
	public class ServiceConnectionOptionsParser : IServiceConnectionOptionsParser
	{
		#region Methods

		protected internal virtual ServiceConnectionOptions CreateServiceConnectionOptions(IDictionary<string, string> dictionary)
		{
			if(dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));

			var ldapConnectionOptions = new ServiceConnectionOptions();

			var key = nameof(ServiceConnectionOptions.AuthenticationKey);
			if(dictionary.TryGetValue(key, out var value))
			{
				ldapConnectionOptions.AuthenticationKey = value;
				dictionary.Remove(key);
			}

			key = nameof(ServiceConnectionOptions.OperationPathFormat);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapConnectionOptions.OperationPathFormat = value;
				dictionary.Remove(key);
			}

			key = nameof(ServiceConnectionOptions.OperationsPath);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapConnectionOptions.OperationsPath = value;
				dictionary.Remove(key);
			}

			key = nameof(ServiceConnectionOptions.Origin);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapConnectionOptions.Origin = new Uri(value, UriKind.Absolute);
				dictionary.Remove(key);
			}

			key = nameof(ServiceConnectionOptions.ProcessPath);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapConnectionOptions.ProcessPath = value;
				dictionary.Remove(key);
			}

			key = nameof(ServiceConnectionOptions.ProcessWithResultPath);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapConnectionOptions.ProcessWithResultPath = value;
				dictionary.Remove(key);
			}

			key = nameof(ServiceConnectionOptions.Timeout);
			if(dictionary.TryGetValue(key, out value))
			{
				ldapConnectionOptions.Timeout = TimeSpan.Parse(value, null);
				dictionary.Remove(key);
			}

			return ldapConnectionOptions;
		}

		public virtual ServiceConnectionOptions Parse(string value)
		{
			if(value == null)
				return null;

			try
			{
				var dictionary = this.ParseToDictionary(value);

				var serviceConnectionOptions = this.CreateServiceConnectionOptions(dictionary);

				if(dictionary.Any())
					throw new InvalidOperationException($"The following keys/properties are not allowed: {string.Join(", ", dictionary.Keys)}");

				return serviceConnectionOptions;
			}
			catch(Exception exception)
			{
				throw new InvalidOperationException("Could not parse to service-connection-options.", exception);
			}
		}

		protected internal virtual IDictionary<string, string> ParseToDictionary(string value)
		{
			var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

			// ReSharper disable InvertIf
			if(value != null)
			{
				foreach(var keyValuePair in value.Trim().Split(';').Select(keyValuePair => keyValuePair.Trim()).Where(keyValuePair => !string.IsNullOrWhiteSpace(keyValuePair)))
				{
					var parts = keyValuePair.Split(new[] { '=' }, 2).Select(part => part.Trim()).ToArray();
					dictionary.Add(parts[0], parts.Length > 1 ? parts[1] : null);
				}
			}
			// ReSharper restore InvertIf

			return dictionary;
		}

		#endregion
	}
}