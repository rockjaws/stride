using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProjectTasks_ProjectTaskId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ProjectTaskId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ProjectTaskId",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TaskId",
                table: "Notifications",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProjectTasks_TaskId",
                table: "Notifications",
                column: "TaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_ProjectTasks_TaskId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TaskId",
                table: "Notifications");

            migrationBuilder.AddColumn<int>(
                name: "ProjectTaskId",
                table: "Notifications",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ProjectTaskId",
                table: "Notifications",
                column: "ProjectTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_ProjectTasks_ProjectTaskId",
                table: "Notifications",
                column: "ProjectTaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id");
        }
    }
}
