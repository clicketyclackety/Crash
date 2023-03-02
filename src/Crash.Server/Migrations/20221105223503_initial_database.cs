using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crash.Server.Migrations
{
	public partial class initial_database : Migration
	{
		internal const string TABLE_NAME = $"Changes";
		internal const string PK_NAME = $"PK_{TABLE_NAME}";

		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: TABLE_NAME,
				columns: table => new
				{
					Id = table.Column<Guid>(type: "TEXT", nullable: false),
					Stamp = table.Column<DateTime>(type: "TEXT", nullable: false),
					Owner = table.Column<string>(type: "TEXT", nullable: true),
					Payload = table.Column<string>(type: "TEXT", nullable: true),
					Action = table.Column<int>(type: "INTEGER", nullable: false),
				},
				constraints: table =>
				{
					// We don't want to store changes by ID anymore!
					// We want to collate them by ID!
					table.PrimaryKey(PK_NAME, x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: TABLE_NAME);
		}
	}
}
