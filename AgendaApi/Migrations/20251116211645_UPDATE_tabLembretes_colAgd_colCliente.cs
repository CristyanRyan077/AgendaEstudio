using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgendaApi.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_tabLembretes_colAgd_colCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LembreteTipo",
                table: "Lembretes");

            migrationBuilder.CreateIndex(
                name: "IX_Lembretes_AgendamentoId",
                table: "Lembretes",
                column: "AgendamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lembretes_Agendamentos_AgendamentoId",
                table: "Lembretes",
                column: "AgendamentoId",
                principalTable: "Agendamentos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lembretes_Clientes_ClienteId",
                table: "Lembretes",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lembretes_Agendamentos_AgendamentoId",
                table: "Lembretes");

            migrationBuilder.DropForeignKey(
                name: "FK_Lembretes_Clientes_ClienteId",
                table: "Lembretes");

            migrationBuilder.DropIndex(
                name: "IX_Lembretes_AgendamentoId",
                table: "Lembretes");

            migrationBuilder.AddColumn<string>(
                name: "LembreteTipo",
                table: "Lembretes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
