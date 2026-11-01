using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalPetTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome_usuario = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    senha_hash = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    perfil = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    criado_em = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id_usuario);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_email",
                table: "Usuario",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
