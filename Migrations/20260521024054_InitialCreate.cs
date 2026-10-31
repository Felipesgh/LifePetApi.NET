using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifePetApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TUTORES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUTORES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PETS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RACA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TUTOR_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PETS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PETS_TUTORES_TUTOR_ID",
                        column: x => x.TUTOR_ID,
                        principalTable: "TUTORES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CONSULTAS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DATA_HORA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    VETERINARIO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    MOTIVO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    PET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONSULTAS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CONSULTAS_PETS_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PETS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HISTORICOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DATA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    TIPO = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    PET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HISTORICOS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_HISTORICOS_PETS_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PETS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MEDICAMENTOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DOSAGEM = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FREQUENCIA = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DATA_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_FIM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEDICAMENTOS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MEDICAMENTOS_PETS_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PETS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VACINAS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DATA_APLICACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_PROXIMA_DOSE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PET_ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VACINAS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VACINAS_PETS_PET_ID",
                        column: x => x.PET_ID,
                        principalTable: "PETS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTAS_PET_ID",
                table: "CONSULTAS",
                column: "PET_ID");

            migrationBuilder.CreateIndex(
                name: "IX_HISTORICOS_PET_ID",
                table: "HISTORICOS",
                column: "PET_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MEDICAMENTOS_PET_ID",
                table: "MEDICAMENTOS",
                column: "PET_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PETS_TUTOR_ID",
                table: "PETS",
                column: "TUTOR_ID");

            migrationBuilder.CreateIndex(
                name: "IX_VACINAS_PET_ID",
                table: "VACINAS",
                column: "PET_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CONSULTAS");

            migrationBuilder.DropTable(
                name: "HISTORICOS");

            migrationBuilder.DropTable(
                name: "MEDICAMENTOS");

            migrationBuilder.DropTable(
                name: "VACINAS");

            migrationBuilder.DropTable(
                name: "PETS");

            migrationBuilder.DropTable(
                name: "TUTORES");
        }
    }
}
