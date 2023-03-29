using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Service.Models;

namespace UnitTests.Models
{
	[TestClass]
	public class InMemoryOperationRepositoryTest
	{
		#region Methods

		protected internal virtual async Task<IOperation> CreateOperationAsync(DateTimeOffset? end = null, Guid? id = null, object result = null, DateTimeOffset? start = null)
		{
			var operationMock = new Mock<IOperation>();

			operationMock.Setup(operation => operation.End).Returns(end);
			operationMock.Setup(operation => operation.Id).Returns(id ?? Guid.NewGuid());
			operationMock.Setup(operation => operation.Result).Returns(result);
			operationMock.Setup(operation => operation.Start).Returns(start ?? DateTimeOffset.UtcNow);

			return await Task.FromResult(operationMock.Object);
		}

		[TestMethod]
		public async Task Save_IsUpdate_Test()
		{
			var operationRepository = new InMemoryOperationRepository();
			Assert.IsFalse(operationRepository.Operations.Any());

			var operation = await this.CreateOperationAsync();
			await operationRepository.SaveAsync(operation);
			Assert.AreEqual(1, operationRepository.Operations.Count);
			Assert.AreEqual(operation, await operationRepository.GetAsync(operation.Id));

			var updatedOperation = await this.CreateOperationAsync(DateTimeOffset.UtcNow, operation.Id, new object(), operation.Start);
			await operationRepository.SaveAsync(updatedOperation);
			Assert.AreEqual(1, operationRepository.Operations.Count);
			Assert.AreEqual(updatedOperation, await operationRepository.GetAsync(operation.Id));

			var pickedOperation = await operationRepository.GetAsync(operation.Id);
			Assert.IsNotNull(pickedOperation.End);
			Assert.IsNotNull(pickedOperation.Result);
			Assert.AreEqual(operation.Id, pickedOperation.Id);
			Assert.AreEqual(operation.Start, pickedOperation.Start);
		}

		#endregion
	}
}