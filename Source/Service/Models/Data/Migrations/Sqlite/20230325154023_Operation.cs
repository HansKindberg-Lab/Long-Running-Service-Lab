#nullable disable
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Models.Data.Migrations.Sqlite
{
	/// <inheritdoc />
	public partial class Operation : Migration
	{
		#region Methods

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Operations");
		}

		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Operations",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "TEXT", nullable: false),
					End = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
					Result = table.Column<string>(type: "TEXT", nullable: true),
					ResultType = table.Column<string>(type: "TEXT", nullable: true),
					Start = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
				},
				constraints: table => { table.PrimaryKey("PK_Operations", x => x.Id); });
		}

		#endregion
	}
}