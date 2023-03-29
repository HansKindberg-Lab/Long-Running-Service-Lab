using Client.Models;
using Client.Models.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages
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
		public virtual IList<IOperation> Operations { get; } = new List<IOperation>();
		protected internal virtual IServiceClient ServiceClient { get; }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> OnGet()
		{
			try
			{
				foreach(var operation in await this.ServiceClient.Operations())
				{
					this.Operations.Add(operation);
				}
			}
			catch(Exception exception)
			{
				this.Operations.Clear();
				this.Exception = exception;
			}

			return this.Page();
		}

		#endregion
	}
}