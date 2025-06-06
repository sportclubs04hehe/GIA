using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Dm_ThuocTinh_Added_Db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ThuocTinhId",
                table: "MatHang",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Dm_ThuocTinh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Stt = table.Column<string>(type: "text", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: false),
                    Ten = table.Column<string>(type: "text", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    DinhDang = table.Column<string>(type: "text", nullable: true),
                    Width = table.Column<string>(type: "text", nullable: true),
                    CongThuc = table.Column<string>(type: "text", nullable: true),
                    CanChinhCot = table.Column<string>(type: "text", nullable: true),
                    NgayHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayHetHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThuocTinhChaId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dm_ThuocTinh", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dm_ThuocTinh_Dm_ThuocTinh_ThuocTinhChaId",
                        column: x => x.ThuocTinhChaId,
                        principalTable: "Dm_ThuocTinh",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatHang_ThuocTinhId",
                table: "MatHang",
                column: "ThuocTinhId");

            migrationBuilder.CreateIndex(
                name: "IX_Dm_ThuocTinh_ThuocTinhChaId",
                table: "Dm_ThuocTinh",
                column: "ThuocTinhChaId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatHang_Dm_ThuocTinh_ThuocTinhId",
                table: "MatHang",
                column: "ThuocTinhId",
                principalTable: "Dm_ThuocTinh",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatHang_Dm_ThuocTinh_ThuocTinhId",
                table: "MatHang");

            migrationBuilder.DropTable(
                name: "Dm_ThuocTinh");

            migrationBuilder.DropIndex(
                name: "IX_MatHang_ThuocTinhId",
                table: "MatHang");

            migrationBuilder.DropColumn(
                name: "ThuocTinhId",
                table: "MatHang");
        }
    }
}
