using System.Net;
using System.Text.Json;
using Client.Models.Configuration;
using Client.Models.Json.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Client.Models.ServiceClients
{
	public class ServiceClient : IServiceClient
	{
		#region Fields

		private static JsonSerializerOptions _jsonSerializerOptions;

		#endregion

		#region Constructors

		public ServiceClient(IConfiguration configuration, HttpClient httpClient, IServiceConnectionOptionsParser serviceConnectionOptionsParser)
		{
			if(configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			if(httpClient == null)
				throw new ArgumentNullException(nameof(httpClient));

			if(serviceConnectionOptionsParser == null)
				throw new ArgumentNullException(nameof(serviceConnectionOptionsParser));

			var connectionString = configuration.GetConnectionString(ConnectionStringNames.Service) ?? throw new InvalidOperationException($"Could not find a connection-string with name \"{ConnectionStringNames.Service}\".");
			var serviceConnectionOptions = serviceConnectionOptionsParser.Parse(connectionString) ?? throw new InvalidOperationException("The parsed service-connection-options is null.");

			if(serviceConnectionOptions.AuthenticationKey != null)
				httpClient.DefaultRequestHeaders.Add("authentication-key", serviceConnectionOptions.AuthenticationKey);

			httpClient.BaseAddress = serviceConnectionOptions.Origin;

			if(serviceConnectionOptions.Timeout != null)
				httpClient.Timeout = serviceConnectionOptions.Timeout.Value;

			this.HttpClient = httpClient;
			this.ServiceConnectionOptions = serviceConnectionOptions;
		}

		#endregion

		#region Properties

		protected virtual HttpClient HttpClient { get; }

		protected internal virtual JsonSerializerOptions JsonSerializerOptions
		{
			get
			{
				if(_jsonSerializerOptions == null)
				{
					var jsonSerializerOptions = new JsonSerializerOptions();
					jsonSerializerOptions.SetDefaults();

					_jsonSerializerOptions = jsonSerializerOptions;
				}

				return _jsonSerializerOptions;
			}
		}

		protected virtual ServiceConnectionOptions ServiceConnectionOptions { get; }

		#endregion

		#region Methods

		protected internal virtual async Task<string> GetOperationPathAsync(IOperation operation)
		{
			if(operation == null)
				throw new ArgumentNullException(nameof(operation));

			return await Task.FromResult($"/Operation?Id={operation.Id}");
		}

		public virtual async Task<IOperation> Operation(Guid? id)
		{
			var response = await this.HttpClient.GetAsync(string.Format(null, this.ServiceConnectionOptions.OperationPathFormat, id));

			var responseContent = await response.Content.ReadAsStringAsync();

			if(!response.IsSuccessStatusCode)
				this.ThrowHttpRequestException(response, responseContent);

			var operation = JsonSerializer.Deserialize<Operation>(responseContent, this.JsonSerializerOptions);

			operation.HttpStatusCode = response.StatusCode;
			operation.Path = await this.GetOperationPathAsync(operation);
			operation.RetryAfterDate = response.Headers.RetryAfter?.Date;
			operation.RetryAfterSeconds = response.Headers.RetryAfter?.Delta?.TotalSeconds;

			await this.ResolveOperationResultAsync(operation);

			return await Task.FromResult(operation);
		}

		public virtual async Task<IEnumerable<IOperation>> Operations()
		{
			var response = await this.HttpClient.GetAsync(this.ServiceConnectionOptions.OperationsPath);

			var responseContent = await response.Content.ReadAsStringAsync();

			if(!response.IsSuccessStatusCode)
				this.ThrowHttpRequestException(response, responseContent);

			var operations = JsonSerializer.Deserialize<List<Operation>>(responseContent, this.JsonSerializerOptions);

			foreach(var operation in operations)
			{
				operation.Path = await this.GetOperationPathAsync(operation);

				await this.ResolveOperationResultAsync(operation);
			}

			return await Task.FromResult(operations);
		}

		public virtual async Task<IOperation> Process(TimeSpan? duration)
		{
			var response = await this.HttpClient.PostAsync($"{this.ServiceConnectionOptions.ProcessPath}{(duration != null ? $"?Duration={duration}" : null)}", null);

			var responseContent = await response.Content.ReadAsStringAsync();

			if(response.StatusCode != HttpStatusCode.Accepted)
				this.ThrowHttpRequestException(response, responseContent);

			var operation = JsonSerializer.Deserialize<Operation>(responseContent, this.JsonSerializerOptions);

			operation.HttpStatusCode = response.StatusCode;
			operation.Location = response.Headers.Location?.ToString();
			operation.Path = await this.GetOperationPathAsync(operation);

			await this.ResolveOperationResultAsync(operation);

			return await Task.FromResult(operation);
		}

		public virtual async Task<IDurationResult> ProcessWithResult(TimeSpan? duration)
		{
			var response = await this.HttpClient.PostAsync($"{this.ServiceConnectionOptions.ProcessWithResultPath}{(duration != null ? $"?Duration={duration}" : null)}", null);

			var responseContent = await response.Content.ReadAsStringAsync();

			if(!response.IsSuccessStatusCode)
				this.ThrowHttpRequestException(response, responseContent);

			var result = JsonSerializer.Deserialize<DurationResult>(responseContent, this.JsonSerializerOptions);

			return await Task.FromResult(result);
		}

		protected internal virtual async Task ResolveOperationResultAsync(Operation operation)
		{
			if(operation == null)
				throw new ArgumentNullException(nameof(operation));

			var result = operation.Result?.ToString();

			if(result == null)
				return;

			await Task.CompletedTask;

			operation.Result = JsonSerializer.Deserialize<DurationResult>(result, this.JsonSerializerOptions);
		}

		protected internal virtual void ThrowHttpRequestException(HttpResponseMessage httpResponseMessage, string responseContent)
		{
			var problemDetails = string.IsNullOrWhiteSpace(responseContent) ? new ProblemDetails { Title = "Error calling service." } : JsonSerializer.Deserialize<ProblemDetails>(responseContent);

			throw new HttpRequestException($"{httpResponseMessage.StatusCode}: {problemDetails.Detail ?? problemDetails.Title}");
		}

		#endregion
	}
}