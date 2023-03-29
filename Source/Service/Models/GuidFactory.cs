using System;

namespace Service.Models
{
	public class GuidFactory : IGuidFactory
	{
		#region Methods

		public virtual Guid Create()
		{
			return Guid.NewGuid();
		}

		#endregion
	}
}