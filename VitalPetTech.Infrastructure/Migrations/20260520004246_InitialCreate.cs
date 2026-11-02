using System;
using Microsoft.EntityFrameworkCore.Migrations;
using VitalPetTech.Infrastructure.Data;
#nullable disable

namespace VitalPetTech.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tutor",
                columns: table => new
                {
                    id_tutor = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome_tutor = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    cpf_tutor = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    telefone_tutor = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: false),
                    endereco_tutor = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutor", x => x.id_tutor);
                });

            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    id_pet = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nome_pet = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    data_nascimento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    peso_pet = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    especie_pet = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    id_tutor = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.id_pet);
                    table.ForeignKey(
                        name: "FK_Pets_Tutor_id_tutor",
                        column: x => x.id_tutor,
                        principalTable: "Tutor",
                        principalColumn: "id_tutor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alerta",
                columns: table => new
                {
                    id_alerta = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    tipo_alerta = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    valor = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    latitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    longitude = table.Column<double>(type: "BINARY_DOUBLE", nullable: true),
                    severidade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    resolvido = table.Column<bool>(type: "NUMBER(1)", nullable: false),
                    timestamp = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    id_pet = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alerta", x => x.id_alerta);
                    table.ForeignKey(
                        name: "FK_Alerta_Pets_id_pet",
                        column: x => x.id_pet,
                        principalTable: "Pets",
                        principalColumn: "id_pet",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evento_Clinico",
                columns: table => new
                {
                    id_evento = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    tipo_evento = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    data_evento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    clinica = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    proximo_retorno = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    id_pet = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evento_Clinico", x => x.id_evento);
                    table.ForeignKey(
                        name: "FK_Evento_Clinico_Pets_id_pet",
                        column: x => x.id_pet,
                        principalTable: "Pets",
                        principalColumn: "id_pet",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alerta_id_pet",
                table: "Alerta",
                column: "id_pet");

            migrationBuilder.CreateIndex(
                name: "IX_Evento_Clinico_id_pet",
                table: "Evento_Clinico",
                column: "id_pet");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_id_tutor",
                table: "Pets",
                column: "id_tutor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alerta");

            migrationBuilder.DropTable(
                name: "Evento_Clinico");

            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropTable(
                name: "Tutor");
        }
    }
}

