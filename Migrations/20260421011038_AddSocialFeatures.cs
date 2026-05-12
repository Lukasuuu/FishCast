using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishCast.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Descricao",
                table: "Peixes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HabitatTipo",
                table: "Peixes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagemUrl",
                table: "Peixes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeCientifico",
                table: "Peixes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Praia",
                table: "Capturas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Titulo",
                table: "Capturas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localidade",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoPescaFavorito",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CapturaLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CapturaId = table.Column<int>(type: "int", nullable: false),
                    UtilizadorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapturaLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapturaLikes_AspNetUsers_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapturaLikes_Capturas_CapturaId",
                        column: x => x.CapturaId,
                        principalTable: "Capturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seguidores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeguidorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeguidoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataSeguimento = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seguidores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seguidores_AspNetUsers_SeguidoId",
                        column: x => x.SeguidoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seguidores_AspNetUsers_SeguidorId",
                        column: x => x.SeguidorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapturaLikes_CapturaId_UtilizadorId",
                table: "CapturaLikes",
                columns: new[] { "CapturaId", "UtilizadorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CapturaLikes_UtilizadorId",
                table: "CapturaLikes",
                column: "UtilizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguidores_SeguidoId",
                table: "Seguidores",
                column: "SeguidoId");

            migrationBuilder.CreateIndex(
                name: "IX_Seguidores_SeguidorId_SeguidoId",
                table: "Seguidores",
                columns: new[] { "SeguidorId", "SeguidoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapturaLikes");

            migrationBuilder.DropTable(
                name: "Seguidores");

            migrationBuilder.DropColumn(
                name: "Descricao",
                table: "Peixes");

            migrationBuilder.DropColumn(
                name: "HabitatTipo",
                table: "Peixes");

            migrationBuilder.DropColumn(
                name: "ImagemUrl",
                table: "Peixes");

            migrationBuilder.DropColumn(
                name: "NomeCientifico",
                table: "Peixes");

            migrationBuilder.DropColumn(
                name: "Praia",
                table: "Capturas");

            migrationBuilder.DropColumn(
                name: "Titulo",
                table: "Capturas");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Localidade",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TipoPescaFavorito",
                table: "AspNetUsers");
        }
    }
}
