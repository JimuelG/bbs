using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBarangayCertificateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResidentId",
                table: "BarangayCertificates",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BarangayCertificates_ResidentId",
                table: "BarangayCertificates",
                column: "ResidentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates");

            migrationBuilder.DropIndex(
                name: "IX_BarangayCertificates_ResidentId",
                table: "BarangayCertificates");

            migrationBuilder.DropColumn(
                name: "ResidentId",
                table: "BarangayCertificates");
        }
    }
}
