using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MentorId",
                table: "Course",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Course_MentorId",
                table: "Course",
                column: "MentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Mentor_MentorId",
                table: "Course",
                column: "MentorId",
                principalTable: "Mentor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Mentor_MentorId",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_MentorId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "Course");
        }
    }
}
