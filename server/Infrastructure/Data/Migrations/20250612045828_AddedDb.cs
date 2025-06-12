using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    Loai = table.Column<int>(type: "integer", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "DonViTinh",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: false),
                    Ten = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_DonViTinh", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoaiGias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: false),
                    Ten = table.Column<string>(type: "text", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiGias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomHangHoa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaNhom = table.Column<string>(type: "text", nullable: false),
                    TenNhom = table.Column<string>(type: "text", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NgayHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayHetHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NhomChaId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomHangHoa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NhomHangHoa_NhomHangHoa_NhomChaId",
                        column: x => x.NhomChaId,
                        principalTable: "NhomHangHoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatHang",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Ma = table.Column<string>(type: "text", nullable: false),
                    Ten = table.Column<string>(type: "text", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NgayHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayHetHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LoaiMatHang = table.Column<int>(type: "integer", nullable: false),
                    MatHangChaId = table.Column<Guid>(type: "uuid", nullable: true),
                    DacTinh = table.Column<string>(type: "text", nullable: true),
                    DonViTinhId = table.Column<Guid>(type: "uuid", nullable: true),
                    ThuocTinhId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatHang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatHang_Dm_ThuocTinh_ThuocTinhId",
                        column: x => x.ThuocTinhId,
                        principalTable: "Dm_ThuocTinh",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatHang_DonViTinh_DonViTinhId",
                        column: x => x.DonViTinhId,
                        principalTable: "DonViTinh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatHang_MatHang_MatHangChaId",
                        column: x => x.MatHangChaId,
                        principalTable: "MatHang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HangHoa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaMatHang = table.Column<string>(type: "text", nullable: false),
                    TenMatHang = table.Column<string>(type: "text", nullable: false),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    NgayHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayHetHieuLuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsHangHoa = table.Column<bool>(type: "boolean", nullable: false),
                    DacTinh = table.Column<string>(type: "text", nullable: true),
                    DonViTinhId = table.Column<Guid>(type: "uuid", nullable: true),
                    NhomHangHoaId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangHoa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HangHoa_DonViTinh_DonViTinhId",
                        column: x => x.DonViTinhId,
                        principalTable: "DonViTinh",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HangHoa_NhomHangHoa_NhomHangHoaId",
                        column: x => x.NhomHangHoaId,
                        principalTable: "NhomHangHoa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThuThapGiaThiTruongs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayThuThap = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HangHoaId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoaiGiaId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoaiNghiepVu = table.Column<int>(type: "integer", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuThapGiaThiTruongs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThuThapGiaThiTruongs_LoaiGias_LoaiGiaId",
                        column: x => x.LoaiGiaId,
                        principalTable: "LoaiGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThuThapGiaThiTruongs_MatHang_HangHoaId",
                        column: x => x.HangHoaId,
                        principalTable: "MatHang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dm_ThuocTinh_ThuocTinhChaId",
                table: "Dm_ThuocTinh",
                column: "ThuocTinhChaId");

            migrationBuilder.CreateIndex(
                name: "IX_HangHoa_DonViTinhId",
                table: "HangHoa",
                column: "DonViTinhId");

            migrationBuilder.CreateIndex(
                name: "IX_HangHoa_NhomHangHoaId",
                table: "HangHoa",
                column: "NhomHangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiGias_Ma",
                table: "LoaiGias",
                column: "Ma",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatHang_DonViTinhId",
                table: "MatHang",
                column: "DonViTinhId");

            migrationBuilder.CreateIndex(
                name: "IX_MatHang_IsDelete",
                table: "MatHang",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_MatHang_LoaiMatHang",
                table: "MatHang",
                column: "LoaiMatHang");

            migrationBuilder.CreateIndex(
                name: "IX_MatHang_Ma_MatHangChaId",
                table: "MatHang",
                columns: new[] { "Ma", "MatHangChaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatHang_MatHangChaId",
                table: "MatHang",
                column: "MatHangChaId");

            migrationBuilder.CreateIndex(
                name: "IX_MatHang_ThuocTinhId",
                table: "MatHang",
                column: "ThuocTinhId");

            migrationBuilder.CreateIndex(
                name: "IX_NhomHangHoa_NhomChaId",
                table: "NhomHangHoa",
                column: "NhomChaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_HangHoaId",
                table: "ThuThapGiaThiTruongs",
                column: "HangHoaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_IsDelete",
                table: "ThuThapGiaThiTruongs",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_LoaiGiaId",
                table: "ThuThapGiaThiTruongs",
                column: "LoaiGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_LoaiNghiepVu",
                table: "ThuThapGiaThiTruongs",
                column: "LoaiNghiepVu");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_NgayThuThap",
                table: "ThuThapGiaThiTruongs",
                column: "NgayThuThap");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_NgayThuThap_HangHoaId_LoaiGiaId_LoaiNg~",
                table: "ThuThapGiaThiTruongs",
                columns: new[] { "NgayThuThap", "HangHoaId", "LoaiGiaId", "LoaiNghiepVu" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HangHoa");

            migrationBuilder.DropTable(
                name: "ThuThapGiaThiTruongs");

            migrationBuilder.DropTable(
                name: "NhomHangHoa");

            migrationBuilder.DropTable(
                name: "LoaiGias");

            migrationBuilder.DropTable(
                name: "MatHang");

            migrationBuilder.DropTable(
                name: "Dm_ThuocTinh");

            migrationBuilder.DropTable(
                name: "DonViTinh");
        }
    }
}
