using Microsoft.EntityFrameworkCore.Migrations;

namespace LojaVirtual.Migrations
{
    public partial class ColaboradoresAtualizacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_colaboradores",
                table: "colaboradores");

            migrationBuilder.RenameTable(
                name: "colaboradores",
                newName: "Colaboradores");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Colaboradores",
                table: "Colaboradores",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Colaboradores",
                table: "Colaboradores");

            migrationBuilder.RenameTable(
                name: "Colaboradores",
                newName: "colaboradores");

            migrationBuilder.AddPrimaryKey(
                name: "PK_colaboradores",
                table: "colaboradores",
                column: "Id");
        }
    }
}
