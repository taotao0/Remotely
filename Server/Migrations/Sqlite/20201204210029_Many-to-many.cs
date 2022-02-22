using Microsoft.EntityFrameworkCore.Migrations;

namespace URemote.Server.Migrations.Sqlite
{
    public partial class Manytomany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionLinks");

            migrationBuilder.CreateTable(
                name: "DeviceGroupRemoteUser",
                columns: table => new
                {
                    DeviceGroupsID = table.Column<string>(type: "TEXT", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceGroupRemoteUser", x => new { x.DeviceGroupsID, x.UsersId });
                    table.ForeignKey(
                        name: "FK_DeviceGroupRemoteUser_DeviceGroups_DeviceGroupsID",
                        column: x => x.DeviceGroupsID,
                        principalTable: "DeviceGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeviceGroupRemoteUser_RemoteUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "RemoteUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceGroupRemoteUser_UsersId",
                table: "DeviceGroupRemoteUser",
                column: "UsersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceGroupRemoteUser");

            migrationBuilder.CreateTable(
                name: "PermissionLinks",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    DeviceGroupID = table.Column<string>(type: "TEXT", nullable: true),
                    UserID = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionLinks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PermissionLinks_DeviceGroups_DeviceGroupID",
                        column: x => x.DeviceGroupID,
                        principalTable: "DeviceGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PermissionLinks_RemoteUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "RemoteUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionLinks_DeviceGroupID",
                table: "PermissionLinks",
                column: "DeviceGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionLinks_UserID",
                table: "PermissionLinks",
                column: "UserID");
        }
    }
}
