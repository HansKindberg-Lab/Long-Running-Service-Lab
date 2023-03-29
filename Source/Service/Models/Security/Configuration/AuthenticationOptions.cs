using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

namespace Service.Models.Security.Configuration
{
	public class AuthenticationOptions
	{
		#region Properties

		public virtual string HeaderName { get; set; } = "authentication-key";

		// I think ISet has worked before. You can try later. But for the time being we use a List instead.
		//public virtual ISet<IPAddress> IpAddresses { get; } = new HashSet<IPAddress>();
		public virtual IList<IPAddress> IpAddresses { get; } = new List<IPAddress>();

		// I think ISet has worked before. You can try later. But for the time being we use a List instead.
		//public virtual ISet<IPNetwork> IpNetworks { get; } = new HashSet<IPNetwork>();
		public virtual IList<IPNetwork> IpNetworks { get; } = new List<IPNetwork>();
		public virtual ISet<string> Keys { get; } = new HashSet<string>();
		public virtual AuthenticationMode Mode { get; set; } = AuthenticationMode.All;

		#endregion
	}
}