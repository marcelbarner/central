using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Central.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrespondents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CorrespondentId",
                table: "Documents",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Correspondents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Correspondents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CorrespondentId",
                table: "Documents",
                column: "CorrespondentId");

            migrationBuilder.CreateIndex(
                name: "IX_Correspondents_Name",
                table: "Correspondents",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Correspondents_CorrespondentId",
                table: "Documents",
                column: "CorrespondentId",
                principalTable: "Correspondents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Correspondents_CorrespondentId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "Correspondents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_CorrespondentId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CorrespondentId",
                table: "Documents");
        }
    }
}
