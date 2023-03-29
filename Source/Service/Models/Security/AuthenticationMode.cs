using System;

namespace Service.Models.Security
{
	[Flags]
	public enum AuthenticationMode
	{
		Off = 0,

		/// <summary>
		/// Https is required during authentication.
		/// </summary>
		Https = 1,

		/// <summary>
		/// Remote IP-address validation is performed.
		/// </summary>
		Ip = 2,

		/// <summary>
		/// Authentication-key validation is performed.
		/// </summary>
		Key = 4,
		All = Https | Ip | Key
	}
}