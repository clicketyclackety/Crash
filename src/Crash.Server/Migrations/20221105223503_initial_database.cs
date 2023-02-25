﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crash.Server.Migrations
{
	public partial class initial_database : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Changes",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "TEXT", nullable: false),
					Stamp = table.Column<DateTime>(type: "TEXT", nullable: false),
					Owner = table.Column<string>(type: "TEXT", nullable: true),
					Payload = table.Column<string>(type: "TEXT", nullable: true),
					Action = table.Column<int>(type: "TEXT", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Changes", x => x.Id);
				});
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Changes");
		}
	}
}
