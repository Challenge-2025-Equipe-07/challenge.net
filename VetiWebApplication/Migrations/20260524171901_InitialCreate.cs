using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetiWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_MEDICAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NmMedicamento = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DsDosagem = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DsFrequencia = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_MEDICAMENTO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_TUTOR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NmTutor = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DsCpf = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    DsEmail = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    DsTelefone = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TUTOR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_VETERINARIO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DsEmail = table.Column<string>(type: "NVARCHAR2(450)", nullable: false),
                    DsPassword = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VETERINARIO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_PET",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NmPet = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DsEspecie = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DsRaca = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    NrIdade = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    StCastrado = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TutorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PET", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_PET_TB_TUTOR_TutorId",
                        column: x => x.TutorId,
                        principalTable: "TB_TUTOR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_CONSULTA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DtConsulta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    TpEvento = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Notificar = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VeterinarioId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CONSULTA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_CONSULTA_TB_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "TB_PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_CONSULTA_TB_VETERINARIO_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "TB_VETERINARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_EXAME",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DsDocumento = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DtRealizacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DsDiagnostico = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ConsultaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_EXAME", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_EXAME_TB_CONSULTA_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "TB_CONSULTA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_EXAME_MEDICAMENTO",
                columns: table => new
                {
                    ExameId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MedicamentoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QtMedicamento = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_EXAME_MEDICAMENTO", x => new { x.ExameId, x.MedicamentoId });
                    table.ForeignKey(
                        name: "FK_TB_EXAME_MEDICAMENTO_TB_EXAME_ExameId",
                        column: x => x.ExameId,
                        principalTable: "TB_EXAME",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_EXAME_MEDICAMENTO_TB_MEDICAMENTO_MedicamentoId",
                        column: x => x.MedicamentoId,
                        principalTable: "TB_MEDICAMENTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONSULTA_PetId",
                table: "TB_CONSULTA",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONSULTA_VeterinarioId",
                table: "TB_CONSULTA",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_EXAME_ConsultaId",
                table: "TB_EXAME",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_EXAME_MEDICAMENTO_MedicamentoId",
                table: "TB_EXAME_MEDICAMENTO",
                column: "MedicamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PET_TutorId",
                table: "TB_PET",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TUTOR_DsCpf",
                table: "TB_TUTOR",
                column: "DsCpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_TUTOR_DsEmail",
                table: "TB_TUTOR",
                column: "DsEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_VETERINARIO_DsEmail",
                table: "TB_VETERINARIO",
                column: "DsEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_EXAME_MEDICAMENTO");

            migrationBuilder.DropTable(
                name: "TB_EXAME");

            migrationBuilder.DropTable(
                name: "TB_MEDICAMENTO");

            migrationBuilder.DropTable(
                name: "TB_CONSULTA");

            migrationBuilder.DropTable(
                name: "TB_PET");

            migrationBuilder.DropTable(
                name: "TB_VETERINARIO");

            migrationBuilder.DropTable(
                name: "TB_TUTOR");
        }
    }
}
