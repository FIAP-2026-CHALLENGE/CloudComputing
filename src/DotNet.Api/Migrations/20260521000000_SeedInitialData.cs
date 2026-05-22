using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNet.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "T_CP_RESPONSAVEIS",
                columns: new[] { "ID", "CPF", "CREATED_AT", "EMAIL", "IS_ACTIVE", "NAME", "PHONE" },
                values: new object[,]
                {
                    { 1, "123.456.789-00", new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "ana.souza@email.com",     1, "Ana Paula Souza",      "(11) 91234-5678" },
                    { 2, "987.654.321-00", new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "carlos.lima@email.com",   1, "Carlos Eduardo Lima",  "(21) 99876-5432" }
                });

            migrationBuilder.InsertData(
                table: "T_CP_ANIMAIS",
                columns: new[] { "ID", "BIRTH_DATE", "BREED", "CREATED_AT", "IS_ACTIVE", "NAME", "NICKNAME", "RGA", "SEX", "SPECIES", "RESPONSAVEL_ID", "WEIGHT" },
                values: new object[,]
                {
                    { 1, new DateTime(2020, 5, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Labrador", new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Bolinha", "Boli",  "RGA-001", "MALE",   "DOG", 1, 28.5m },
                    { 2, new DateTime(2021, 8,  3, 0, 0, 0, 0, DateTimeKind.Utc), "Siamese",  new DateTime(2025, 2, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Mia",     "Miau",  "RGA-002", "FEMALE", "CAT", 2,  4.2m }
                });

            migrationBuilder.InsertData(
                table: "T_CP_CARE_EVENTS",
                columns: new[] { "ID", "COMPLETED_DATE", "CREATED_AT", "DESCRIPTION", "IS_ACTIVE", "NOTES", "ANIMAL_ID", "PRIORITY", "SCHEDULED_DATE", "STATUS", "TITLE", "TYPE" },
                values: new object[,]
                {
                    {
                        1,
                        new DateTime(2025, 6,  1, 9, 30, 0, 0, DateTimeKind.Utc),
                        new DateTime(2025, 5, 28, 0,  0, 0, 0, DateTimeKind.Utc),
                        "Aplicação da vacina polivalente V10 anual obrigatória.",
                        1,
                        "Bolinha reagiu bem. Próxima dose em junho de 2026.",
                        1, "HIGH",
                        new DateTime(2025, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc),
                        "COMPLETED", "Vacina V10 anual", "VACCINE"
                    },
                    {
                        2,
                        null,
                        new DateTime(2025, 7,  1, 0,  0, 0, 0, DateTimeKind.Utc),
                        "Consulta de rotina com exames de sangue e urina.",
                        1,
                        "Agendar jejum de 8h antes da consulta.",
                        2, "MEDIUM",
                        new DateTime(2025, 7, 15, 14, 0, 0, 0, DateTimeKind.Utc),
                        "PENDING", "Check-up semestral Mia", "CHECKUP"
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "T_CP_CARE_EVENTS", keyColumn: "ID", keyValues: new object[] { 1, 2 });
            migrationBuilder.DeleteData(table: "T_CP_ANIMAIS",        keyColumn: "ID", keyValues: new object[] { 1, 2 });
            migrationBuilder.DeleteData(table: "T_CP_RESPONSAVEIS",      keyColumn: "ID", keyValues: new object[] { 1, 2 });
        }
    }
}
