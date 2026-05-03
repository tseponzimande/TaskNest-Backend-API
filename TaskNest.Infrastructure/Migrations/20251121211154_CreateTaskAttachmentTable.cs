using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskNest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTaskAttachmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
            name: "TaskAttachments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                OriginalFileName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                FileSize = table.Column<long>(type: "bigint", nullable: false),
                FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                TaskItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TaskAttachments", x => x.Id);
                table.ForeignKey(
                    name: "FK_TaskAttachments_Tasks_TaskItemId",
                    column: x => x.TaskItemId,
                    principalTable: "Tasks",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_TaskAttachments_AspNetUsers_ApplicationUserId",
                    column: x => x.ApplicationUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_TaskItemId",
                table: "TaskAttachments",
                column: "TaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_ApplicationUserId",
                table: "TaskAttachments",
                column: "ApplicationUserId");
        }
        

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TaskAttachments");
        }
    }
}
