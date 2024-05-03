using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHub.Migrations.DataDb
{
    public partial class InitialCreatedotnet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExternalId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RelationshipUserAndRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idUser = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    IdRole = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipUserAndRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeviceInterfaceItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DataType = table.Column<int>(type: "INTEGER", nullable: false),
                    Control = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceInterfaceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceInterfaceItem_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GroupEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupEntities_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RelationshipGroupsAndroles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    idGroup = table.Column<int>(type: "INTEGER", nullable: false),
                    NameGroup = table.Column<string>(type: "TEXT", nullable: false),
                    IdRole = table.Column<string>(type: "TEXT", nullable: false),
                    GroupEntityId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelationshipGroupsAndroles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelationshipGroupsAndroles_GroupEntities_GroupEntityId",
                        column: x => x.GroupEntityId,
                        principalTable: "GroupEntities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceInterfaceItem_DeviceId",
                table: "DeviceInterfaceItem",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupEntities_DeviceId",
                table: "GroupEntities",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_RelationshipGroupsAndroles_GroupEntityId",
                table: "RelationshipGroupsAndroles",
                column: "GroupEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceInterfaceItem");

            migrationBuilder.DropTable(
                name: "RelationshipGroupsAndroles");

            migrationBuilder.DropTable(
                name: "RelationshipUserAndRoles");

            migrationBuilder.DropTable(
                name: "GroupEntities");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
