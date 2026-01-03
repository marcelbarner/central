using System;

using Microsoft.EntityFrameworkCore.Migrations;

using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Central.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    TriggerState = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessExecutions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessExecutions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessExecutions_ProcessDefinitions_ProcessDefinitionId",
                        column: x => x.ProcessDefinitionId,
                        principalTable: "ProcessDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessDefinitionId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StepType = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    AzureEndpoint = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AzureApiKey = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AzureModelOrDeployment = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Prompt = table.Column<string>(type: "text", nullable: true),
                    Configuration = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessingSteps_ProcessDefinitions_ProcessDefinitionId",
                        column: x => x.ProcessDefinitionId,
                        principalTable: "ProcessDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessExecutionSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessExecutionId = table.Column<long>(type: "bigint", nullable: false),
                    StepName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StepType = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    Output = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessExecutionSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessExecutionSteps_ProcessExecutions_ProcessExecutionId",
                        column: x => x.ProcessExecutionId,
                        principalTable: "ProcessExecutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDefinitions_Enabled",
                table: "ProcessDefinitions",
                column: "Enabled");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDefinitions_Name",
                table: "ProcessDefinitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessDefinitions_TriggerState",
                table: "ProcessDefinitions",
                column: "TriggerState");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExecutions_DocumentId",
                table: "ProcessExecutions",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExecutions_ProcessDefinitionId",
                table: "ProcessExecutions",
                column: "ProcessDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExecutions_StartedAt",
                table: "ProcessExecutions",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExecutions_Status",
                table: "ProcessExecutions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExecutionSteps_ProcessExecutionId",
                table: "ProcessExecutionSteps",
                column: "ProcessExecutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExecutionSteps_ProcessExecutionId_Order",
                table: "ProcessExecutionSteps",
                columns: new[] { "ProcessExecutionId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessExecutionSteps_Status",
                table: "ProcessExecutionSteps",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingSteps_ProcessDefinitionId",
                table: "ProcessingSteps",
                column: "ProcessDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingSteps_ProcessDefinitionId_Order",
                table: "ProcessingSteps",
                columns: new[] { "ProcessDefinitionId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessExecutionSteps");

            migrationBuilder.DropTable(
                name: "ProcessingSteps");

            migrationBuilder.DropTable(
                name: "ProcessExecutions");

            migrationBuilder.DropTable(
                name: "ProcessDefinitions");
        }
    }
}