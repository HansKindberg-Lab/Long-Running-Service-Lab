using Client.Models;
using Client.Models.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Operation
{
	public class IndexModel : PageModel
	{
		#region Constructors

		public IndexModel(IServiceClient serviceClient)
		{
			this.ServiceClient = serviceClient ?? throw new ArgumentNullException(nameof(serviceClient));
		}

		#endregion

		#region Properties

		public virtual Exception Exception { get; set; }
		public virtual string Id { get; set; }
		public virtual IOperation Operation { get; set; }
		protected internal virtual IServiceClient ServiceClient { get; }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> OnGet(string id)
		{
			this.Id = id;

			if(!string.IsNullOrEmpty(id))
			{
				try
				{
					if(!Guid.TryParse(id, out var guid))
						throw new FormatException($"The id \"{id}\" is not a valid guid.");

					this.Operation = await this.ServiceClient.Operation(guid);
				}
				catch(Exception exception)
				{
					this.Exception = exception;
					this.Operation = null;
				}
			}

			return this.Page();
		}

		#endregion
	}
}