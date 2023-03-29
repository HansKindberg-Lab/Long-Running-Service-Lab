using Microsoft.EntityFrameworkCore;

namespace Service.Models.Data
{
	public abstract class OperationContext : DbContext, IOperationContext
	{
		#region Constructors

		protected OperationContext(DbContextOptions options) : base(options) { }

		#endregion

		#region Properties

		public virtual DbSet<Entities.Operation> Operations { get; set; }

		#endregion
	}

	public abstract class OperationContext<T> : OperationContext where T : OperationContext
	{
		#region Constructors

		protected OperationContext(DbContextOptions<T> options) : base(options) { }

		#endregion
	}
}