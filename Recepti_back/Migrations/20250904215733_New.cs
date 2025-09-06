using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recepti.Migrations
{
    /// <inheritdoc />
    public partial class New : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Podkategorije_Kategorije_KategorijaId1",
                table: "Podkategorije");

            migrationBuilder.DropIndex(
                name: "IX_Podkategorije_KategorijaId1",
                table: "Podkategorije");

            migrationBuilder.DropColumn(
                name: "KategorijaId1",
                table: "Podkategorije");

            migrationBuilder.AlterColumn<int>(
                name: "KategorijaId",
                table: "Podkategorije",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "KategorijaId",
                table: "Podkategorije",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "KategorijaId1",
                table: "Podkategorije",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Podkategorije_KategorijaId1",
                table: "Podkategorije",
                column: "KategorijaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Podkategorije_Kategorije_KategorijaId1",
                table: "Podkategorije",
                column: "KategorijaId1",
                principalTable: "Kategorije",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
