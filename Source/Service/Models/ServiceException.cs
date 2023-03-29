using System;
using System.Runtime.Serialization;

namespace Service.Models
{
	public class ServiceException : Exception
	{
		#region Constructors

		public ServiceException() { }
		public ServiceException(string message) : base(message) { }
		public ServiceException(string message, Exception innerException) : base(message, innerException) { }
		protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		#endregion
	}
}