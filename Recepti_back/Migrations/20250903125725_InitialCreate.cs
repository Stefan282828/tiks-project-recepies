using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Recepti.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategorije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategorije", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sastojci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    JedinicaMere = table.Column<string>(type: "text", nullable: false),
                    Kolicina = table.Column<decimal>(type: "numeric(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sastojci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Podkategorije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "text", nullable: false),
                    KategorijaId = table.Column<int>(type: "integer", nullable: true),
                    KategorijaId1 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Podkategorije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Podkategorije_Kategorije_KategorijaId",
                        column: x => x.KategorijaId,
                        principalTable: "Kategorije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Podkategorije_Kategorije_KategorijaId1",
                        column: x => x.KategorijaId1,
                        principalTable: "Kategorije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recepti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Opis = table.Column<string>(type: "text", nullable: false),
                    VremePripreme = table.Column<int>(type: "integer", nullable: false),
                    UputstvoPripreme = table.Column<string>(type: "text", nullable: false),
                    PodKategorijaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recepti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recepti_Podkategorije_PodKategorijaId",
                        column: x => x.PodKategorijaId,
                        principalTable: "Podkategorije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceptSastojci",
                columns: table => new
                {
                    ReceptId = table.Column<int>(type: "integer", nullable: false),
                    SastojakId = table.Column<int>(type: "integer", nullable: false),
                    Kolicina = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    JedinicaMere = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_Podkategorije_KategorijaId",
                table: "Podkategorije",
                column: "KategorijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Podkategorije_KategorijaId1",
                table: "Podkategorije",
                column: "KategorijaId1");

            migrationBuilder.CreateIndex(
                name: "IX_Recepti_PodKategorijaId",
                table: "Recepti",
                column: "PodKategorijaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceptSastojci_SastojakId",
                table: "ReceptSastojci",
                column: "SastojakId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceptSastojci");

            migrationBuilder.DropTable(
                name: "Recepti");

            migrationBuilder.DropTable(
                name: "Sastojci");

            migrationBuilder.DropTable(
                name: "Podkategorije");

            migrationBuilder.DropTable(
                name: "Kategorije");
        }
    }
}
