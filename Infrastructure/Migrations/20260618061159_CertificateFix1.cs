using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CertificateFix1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates");

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

            migrationBuilder.AddForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "Id");
        }
    }
}
