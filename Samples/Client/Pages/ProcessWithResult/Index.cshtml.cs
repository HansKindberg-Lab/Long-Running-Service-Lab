using Client.Models;
using Client.Models.ServiceClients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Client.Pages.ProcessWithResult
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
		public virtual IDurationResult Result { get; set; }
		protected internal virtual IServiceClient ServiceClient { get; }
		public virtual bool ThrowException { get; set; }

		#endregion

		#region Methods

		public virtual async Task<IActionResult> OnPost(TimeSpan? duration, bool throwException)
		{
			this.Duration = duration;
			this.ThrowException = throwException;

			try
			{
				this.Result = await this.ServiceClient.ProcessWithResult(duration, throwException);
			}
			catch(Exception exception)
			{
				this.Exception = exception;
				this.Result = null;
			}

			return this.Page();
		}

		#endregion
	}
}