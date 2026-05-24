using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetiWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddTratamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_TRATAMENTO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DsDiagnostico = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    DtInicio = table.Column<string>(type: "NVARCHAR2(10)", nullable: false),
                    DtRetornoPrevisto = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    DsObservacao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TRATAMENTO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_TRATAMENTO_TB_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "TB_PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_TRATAMENTO_MEDICAMENTO",
                columns: table => new
                {
                    TratamentoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    MedicamentoId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    QtMedicamento = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DsInstrucao = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TRATAMENTO_MEDICAMENTO", x => new { x.TratamentoId, x.MedicamentoId });
                    table.ForeignKey(
                        name: "FK_TB_TRATAMENTO_MEDICAMENTO_TB_MEDICAMENTO_MedicamentoId",
                        column: x => x.MedicamentoId,
                        principalTable: "TB_MEDICAMENTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_TRATAMENTO_MEDICAMENTO_TB_TRATAMENTO_TratamentoId",
                        column: x => x.TratamentoId,
                        principalTable: "TB_TRATAMENTO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_TRATAMENTO_PetId",
                table: "TB_TRATAMENTO",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TRATAMENTO_MEDICAMENTO_MedicamentoId",
                table: "TB_TRATAMENTO_MEDICAMENTO",
                column: "MedicamentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_TRATAMENTO_MEDICAMENTO");

            migrationBuilder.DropTable(
                name: "TB_TRATAMENTO");
        }
    }
}
