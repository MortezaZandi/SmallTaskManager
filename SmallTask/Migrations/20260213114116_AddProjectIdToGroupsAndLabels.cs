using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmallTask.Migrations
{
    public partial class AddProjectIdToGroupsAndLabels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Labels",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("UPDATE Groups SET ProjectId = 1 WHERE ProjectId IS NULL;");
            migrationBuilder.Sql("UPDATE Labels SET ProjectId = 1 WHERE ProjectId IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Groups",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Labels",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ProjectId",
                table: "Groups",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Labels_ProjectId",
                table: "Labels",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Projects_ProjectId",
                table: "Groups",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Labels_Projects_ProjectId",
                table: "Labels",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Projects_ProjectId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Labels_Projects_ProjectId",
                table: "Labels");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ProjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Labels_ProjectId",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Labels");
        }
    }

}
