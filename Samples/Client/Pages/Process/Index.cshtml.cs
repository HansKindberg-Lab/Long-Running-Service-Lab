using Client.Models;
using Client.Models.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.Process
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

		public virtual TimeSpan? Duration { get; set; }
		public virtual Exception Exception { get; set; }
		public virtual IOperation Operation { get; set; }
		protected internal virtual IServiceClient ServiceClient { get; }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> OnPost(TimeSpan? duration)
		{
			this.Duration = duration;

			try
			{
				this.Operation = await this.ServiceClient.Process(duration);
			}
			catch(Exception exception)
			{
				this.Exception = exception;
				this.Operation = null;
			}

			return this.Page();
		}

		#endregion
	}
}