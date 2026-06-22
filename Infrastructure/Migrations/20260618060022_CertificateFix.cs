using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CertificateFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates");

            migrationBuilder.AlterColumn<int>(
                name: "ResidentId",
                table: "BarangayCertificates",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates");

            migrationBuilder.AlterColumn<int>(
                name: "ResidentId",
                table: "BarangayCertificates",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BarangayCertificates_Residents_ResidentId",
                table: "BarangayCertificates",
                column: "ResidentId",
                principalTable: "Residents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
