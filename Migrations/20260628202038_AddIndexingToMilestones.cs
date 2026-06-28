using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace life_timeline_api.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexingToMilestones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Milestones",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Mood",
                table: "Milestones",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Milestones",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneTags_MilestoneId_TagId",
                table: "MilestoneTags",
                columns: new[] { "MilestoneId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Date",
                table: "Milestones",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Description",
                table: "Milestones",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Mood",
                table: "Milestones",
                column: "Mood");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_Title",
                table: "Milestones",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_MilestoneTags_MilestoneId_TagId",
                table: "MilestoneTags");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Date",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Description",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Mood",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_Title",
                table: "Milestones");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Mood",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Milestones",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
