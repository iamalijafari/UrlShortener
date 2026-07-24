using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using UrlShortener.Infrastructure.Persistence;

#nullable disable

namespace UrlShortener.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260724220000_AddEventDrivenAnalytics")]
public partial class AddEventDrivenAnalytics : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(
                    type: "character varying(200)",
                    maxLength: 200,
                    nullable: false),
                Payload = table.Column<string>(type: "jsonb", nullable: false),
                OccurredAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false),
                PublishedAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: true),
                Attempts = table.Column<int>(type: "integer", nullable: false),
                Error = table.Column<string>(
                    type: "character varying(2000)",
                    maxLength: 2000,
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_outbox_messages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "processed_events",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProcessedAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_processed_events", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "daily_visit_statistics",
            columns: table => new
            {
                ShortUrlId = table.Column<Guid>(type: "uuid", nullable: false),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                ClickCount = table.Column<long>(type: "bigint", nullable: false),
                LastVisitedAtUtc = table.Column<DateTime>(
                    type: "timestamp with time zone",
                    nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_daily_visit_statistics",
                    x => new { x.ShortUrlId, x.Date });
                table.ForeignKey(
                    name: "FK_daily_visit_statistics_short_urls_ShortUrlId",
                    column: x => x.ShortUrlId,
                    principalTable: "short_urls",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_outbox_messages_PublishedAtUtc_OccurredAtUtc",
            table: "outbox_messages",
            columns: new[] { "PublishedAtUtc", "OccurredAtUtc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "daily_visit_statistics");
        migrationBuilder.DropTable(name: "outbox_messages");
        migrationBuilder.DropTable(name: "processed_events");
    }
}
