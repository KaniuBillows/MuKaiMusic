using Microsoft.EntityFrameworkCore.Migrations;

namespace MuKai_Music.Migrations.Music
{
    public partial class musicInfoindex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MusicInfos",
                table: "MusicInfos");

            migrationBuilder.RenameTable(
                name: "MusicInfos",
                newName: "MusicInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MusicInfo",
                table: "MusicInfo",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MusicInfo_Ne_Id",
                table: "MusicInfo",
                column: "Ne_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MusicInfo",
                table: "MusicInfo");

            migrationBuilder.DropIndex(
                name: "IX_MusicInfo_Ne_Id",
                table: "MusicInfo");

            migrationBuilder.RenameTable(
                name: "MusicInfo",
                newName: "MusicInfos");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MusicInfos",
                table: "MusicInfos",
                column: "Id");
        }
    }
}
