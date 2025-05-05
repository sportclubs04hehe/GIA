using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Added_DonViTinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "NhomHangHoa",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "NhomHangHoa",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "NhomHangHoaId",
                table: "HangHoa",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "HangHoa",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "HangHoa",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "DacTinh",
                table: "HangHoa",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DonViTinhId",
                table: "HangHoa",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DonViTinhs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ten = table.Column<string>(type: "text", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NgayHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayHetHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonViTinhs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HangHoa_DonViTinhId",
                table: "HangHoa",
                column: "DonViTinhId");

            migrationBuilder.AddForeignKey(
                name: "FK_HangHoa_DonViTinhs_DonViTinhId",
                table: "HangHoa",
                column: "DonViTinhId",
                principalTable: "DonViTinhs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HangHoa_DonViTinhs_DonViTinhId",
                table: "HangHoa");

            migrationBuilder.DropTable(
                name: "DonViTinhs");

            migrationBuilder.DropIndex(
                name: "IX_HangHoa_DonViTinhId",
                table: "HangHoa");

            migrationBuilder.DropColumn(
                name: "DacTinh",
                table: "HangHoa");

            migrationBuilder.DropColumn(
                name: "DonViTinhId",
                table: "HangHoa");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "NhomHangHoa",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "NhomHangHoa",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NhomHangHoaId",
                table: "HangHoa",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedBy",
                table: "HangHoa",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "HangHoa",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
