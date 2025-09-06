using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Recepti.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceptSastojci");

            migrationBuilder.DropTable(
                name: "Sastojci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sastojci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JedinicaMere = table.Column<string>(type: "text", nullable: false),
                    Kolicina = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sastojci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReceptSastojci",
                columns: table => new
                {
                    ReceptId = table.Column<int>(type: "integer", nullable: false),
                    SastojakId = table.Column<int>(type: "integer", nullable: false),
                    JedinicaMere = table.Column<string>(type: "text", nullable: false),
                    Kolicina = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceptSastojci", x => new { x.ReceptId, x.SastojakId });
                    table.ForeignKey(
                        name: "FK_ReceptSastojci_Recepti_ReceptId",
                        column: x => x.ReceptId,
                        principalTable: "Recepti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceptSastojci_Sastojci_SastojakId",
                        column: x => x.SastojakId,
                        principalTable: "Sastojci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceptSastojci_SastojakId",
                table: "ReceptSastojci",
                column: "SastojakId");
        }
    }
}
