using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Models.Json.Extensions;
using Service.Models.Logging.Extensions;

namespace Service.Models.Data
{
	public class DatabaseOperationRepository : IOperationRepository
	{
		#region Fields

		private static JsonSerializerOptions _jsonSerializerOptions;

		#endregion

		#region Constructors

		public DatabaseOperationRepository(ILoggerFactory loggerFactory, IOperationContextFactory operationContextFactory)
		{
			this.Logger = (loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory))).CreateLogger(this.GetType());
			this.OperationContextFactory = operationContextFactory ?? throw new ArgumentNullException(nameof(operationContextFactory));
		}

		#endregion

		#region Properties

		protected internal virtual JsonSerializerOptions JsonSerializerOptions
		{
			get
			{
				// ReSharper disable InvertIf
				if(_jsonSerializerOptions == null)
				{
					var jsonSerializerOptions = new JsonSerializerOptions();
					jsonSerializerOptions.SetDefaults();
					jsonSerializerOptions.WriteIndented = false;

					_jsonSerializerOptions = jsonSerializerOptions;
				}
				// ReSharper restore InvertIf

				return _jsonSerializerOptions;
			}
		}

		protected internal virtual ILogger Logger { get; }
		protected internal virtual IOperationContextFactory OperationContextFactory { get; }

		#endregion

		#region Methods

		public virtual async Task<bool> DeleteAsync(Guid id)
		{
			using(var operationContext = this.OperationContextFactory.Create())
			{
				operationContext.Operations.Remove(new Entities.Operation { Id = id });

				var numberOfAffectedRecords = await operationContext.SaveChangesAsync();

				return numberOfAffectedRecords > 0;
			}
		}

		public virtual async Task<IOperation> GetAsync(Guid id)
		{
			using(var operationContext = this.OperationContextFactory.Create())
			{
				var entity = await operationContext.Operations.FindAsync(id);

				if(entity == null)
					return null;

				var operation = await this.ToModelAsync(entity);

				return await Task.FromResult(operation);
			}
		}

		public virtual async Task<IEnumerable<IOperation>> ListAsync()
		{
			var operations = new List<IOperation>();

			using(var operationContext = this.OperationContextFactory.Create())
			{
				foreach(var entity in operationContext.Operations)
				{
					operations.Add(await this.ToModelAsync(entity));
				}
			}

			return await Task.FromResult(operations);
		}

		public virtual async Task SaveAsync(IOperation operation)
		{
			if(operation == null)
				throw new ArgumentNullException(nameof(operation));

			if(operation.Id == Guid.Empty)
				throw new ArgumentException("The id can not be an empty guid.", nameof(operation));

			var entityToSave = await this.ToEntityAsync(operation);

			using(var operationContext = this.OperationContextFactory.Create())
			{
				var existingEntity = await operationContext.Operations.FindAsync(operation.Id);

				if(existingEntity == null)
				{
					// Add
					operationContext.Operations.Add(entityToSave);
				}
				else
				{
					// Update
					existingEntity.End = entityToSave.End;
					existingEntity.Result = entityToSave.Result;
					existingEntity.ResultType = entityToSave.ResultType;
					existingEntity.Start = entityToSave.Start;
				}

				await operationContext.SaveChangesAsync();
			}
		}

		protected internal virtual async Task<Entities.Operation> ToEntityAsync(IOperation operation)
		{
			if(operation == null)
				return null;

			var entity = new Entities.Operation
			{
				End = operation.End,
				Id = operation.Id,
				Result = operation.Result == null ? null : JsonSerializer.Serialize(operation.Result, this.JsonSerializerOptions),
				ResultType = operation.Result?.GetType().FullName,
				Start = operation.Start
			};

			return await Task.FromResult(entity);
		}

		protected internal virtual async Task<IOperation> ToModelAsync(Entities.Operation operation)
		{
			if(operation == null)
				return null;

			var model = new Operation
			{
				End = operation.End,
				Id = operation.Id,
				Start = operation.Start
			};

			if(operation.Result != null && operation.ResultType != null)
			{
				try
				{
					var type = Type.GetType(operation.ResultType, true, true);

					model.Result = JsonSerializer.Deserialize(operation.Result, type, this.JsonSerializerOptions);
				}
				catch(Exception exception)
				{
					model.Result = null;
					this.Logger.LogErrorIfEnabled(exception, "Could not.");
				}
			}

			model.Result ??= operation.Result;

			return await Task.FromResult(model);
		}

		#endregion
	}
}