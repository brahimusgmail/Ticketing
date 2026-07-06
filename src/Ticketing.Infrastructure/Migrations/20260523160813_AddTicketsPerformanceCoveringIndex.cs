#nullable disable

namespace Ticketing.Infrastructure.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <inheritdoc />
    public partial class AddTicketsPerformanceCoveringIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Status_CreatedAtUtc_Covering",
                table: "Tickets",
                columns: new[] { "Status", "CreatedAtUtc" })
                .Annotation("SqlServer:Include", new[] { "Id", "Title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_Status_CreatedAtUtc_Covering",
                table: "Tickets");
        }
    }
}
