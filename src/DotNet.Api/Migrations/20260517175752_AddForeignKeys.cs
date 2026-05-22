using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_T_CP_ANIMAIS_RESPONSAVEL_ID",
                table: "T_CP_ANIMAIS",
                column: "RESPONSAVEL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_CP_CARE_EVENTS_PET_ID",
                table: "T_CP_CARE_EVENTS",
                column: "ANIMAL_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_ANIMAIS_PET_ID",
                table: "T_CP_CARE_EVENTS",
                column: "ANIMAL_ID",
                principalTable: "T_CP_ANIMAIS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_T_CP_ANIMAIS_T_CP_RESPONSAVEIS_RESPONSAVEL_ID",
                table: "T_CP_ANIMAIS",
                column: "RESPONSAVEL_ID",
                principalTable: "T_CP_RESPONSAVEIS",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_T_CP_CARE_EVENTS_T_CP_ANIMAIS_PET_ID",
                table: "T_CP_CARE_EVENTS");

            migrationBuilder.DropForeignKey(
                name: "FK_T_CP_ANIMAIS_T_CP_RESPONSAVEIS_RESPONSAVEL_ID",
                table: "T_CP_ANIMAIS");

            migrationBuilder.DropIndex(
                name: "IX_T_CP_ANIMAIS_RESPONSAVEL_ID",
                table: "T_CP_ANIMAIS");

            migrationBuilder.DropIndex(
                name: "IX_T_CP_CARE_EVENTS_PET_ID",
                table: "T_CP_CARE_EVENTS");
        }
    }
}
