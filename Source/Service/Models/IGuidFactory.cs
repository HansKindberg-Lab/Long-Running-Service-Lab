using System;

namespace Service.Models
{
	public interface IGuidFactory
	{
		#region Methods

		Guid Create();

		#endregion
	}
}