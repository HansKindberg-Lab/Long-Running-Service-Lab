#nullable disable
using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Models.Data.Migrations.SqlServer
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
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					End = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
					Result = table.Column<string>(type: "nvarchar(max)", nullable: true),
					ResultType = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Start = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
				},
				constraints: table => { table.PrimaryKey("PK_Operations", x => x.Id); });
		}

		#endregion
	}
}