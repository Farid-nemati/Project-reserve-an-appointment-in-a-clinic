using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project.Data.Migrations
{
    public partial class names : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        { migrationBuilder.CreateTable(
            name: "names",
            columns: table => new
            {
                id = table.Column<int>(type: "int",nullable:false).Annotation("SqlServer:Identity", "1, 1"),
                field = table.Column<string>(type: "nvarchar(max)", nullable: false),
                days = table.Column<string>(type: "nvarchar(max)", nullable: false),
                price = table.Column<int>(type: "int", nullable: false),
                namedoc = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_names", x => x.id);

            });
        }

protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "names");
        }
    }
}
