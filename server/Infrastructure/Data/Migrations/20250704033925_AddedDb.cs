﻿using System;
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
                name: "Dm_HangHoaThiTruong",
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
                    table.PrimaryKey("PK_Dm_HangHoaThiTruong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dm_HangHoaThiTruong_Dm_HangHoaThiTruong_MatHangChaId",
                        column: x => x.MatHangChaId,
                        principalTable: "Dm_HangHoaThiTruong",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dm_HangHoaThiTruong_Dm_ThuocTinh_ThuocTinhId",
                        column: x => x.ThuocTinhId,
                        principalTable: "Dm_ThuocTinh",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dm_HangHoaThiTruong_DonViTinh_DonViTinhId",
                        column: x => x.DonViTinhId,
                        principalTable: "DonViTinh",
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
                    Tuan = table.Column<int>(type: "integer", nullable: true),
                    Nam = table.Column<int>(type: "integer", nullable: false),
                    NgayNhap = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoaiNghiepVu = table.Column<int>(type: "integer", nullable: false),
                    LoaiGiaId = table.Column<Guid>(type: "uuid", nullable: false),
                    NhomHangHoaId = table.Column<Guid>(type: "uuid", nullable: true),
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
                        name: "FK_ThuThapGiaThiTruongs_Dm_HangHoaThiTruong_NhomHangHoaId",
                        column: x => x.NhomHangHoaId,
                        principalTable: "Dm_HangHoaThiTruong",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThuThapGiaThiTruongs_LoaiGias_LoaiGiaId",
                        column: x => x.LoaiGiaId,
                        principalTable: "LoaiGias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThuThapGiaChiTiets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThuThapGiaThiTruongId = table.Column<Guid>(type: "uuid", nullable: false),
                    HangHoaThiTruongId = table.Column<Guid>(type: "uuid", nullable: false),
                    GiaPhoBienKyBaoCao = table.Column<string>(type: "text", nullable: true),
                    GiaBinhQuanKyTruoc = table.Column<decimal>(type: "numeric", nullable: true),
                    GiaBinhQuanKyNay = table.Column<decimal>(type: "numeric", nullable: true),
                    MucTangGiamGiaBinhQuan = table.Column<decimal>(type: "numeric", nullable: true),
                    TyLeTangGiamGiaBinhQuan = table.Column<decimal>(type: "numeric", nullable: true),
                    NguonThongTin = table.Column<string>(type: "text", nullable: true),
                    GhiChu = table.Column<string>(type: "text", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThuThapGiaChiTiets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThuThapGiaChiTiets_Dm_HangHoaThiTruong_HangHoaThiTruongId",
                        column: x => x.HangHoaThiTruongId,
                        principalTable: "Dm_HangHoaThiTruong",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThuThapGiaChiTiets_ThuThapGiaThiTruongs_ThuThapGiaThiTruong~",
                        column: x => x.ThuThapGiaThiTruongId,
                        principalTable: "ThuThapGiaThiTruongs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dm_HangHoaThiTruong_DonViTinhId",
                table: "Dm_HangHoaThiTruong",
                column: "DonViTinhId");

            migrationBuilder.CreateIndex(
                name: "IX_Dm_HangHoaThiTruong_IsDelete",
                table: "Dm_HangHoaThiTruong",
                column: "IsDelete");

            migrationBuilder.CreateIndex(
                name: "IX_Dm_HangHoaThiTruong_LoaiMatHang",
                table: "Dm_HangHoaThiTruong",
                column: "LoaiMatHang");

            migrationBuilder.CreateIndex(
                name: "IX_Dm_HangHoaThiTruong_Ma_MatHangChaId",
                table: "Dm_HangHoaThiTruong",
                columns: new[] { "Ma", "MatHangChaId" },
                unique: true,
                filter: "\"IsDelete\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Dm_HangHoaThiTruong_MatHangChaId",
                table: "Dm_HangHoaThiTruong",
                column: "MatHangChaId");

            migrationBuilder.CreateIndex(
                name: "IX_Dm_HangHoaThiTruong_ThuocTinhId",
                table: "Dm_HangHoaThiTruong",
                column: "ThuocTinhId");

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
                name: "IX_NhomHangHoa_NhomChaId",
                table: "NhomHangHoa",
                column: "NhomChaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaChiTiets_HangHoaThiTruongId",
                table: "ThuThapGiaChiTiets",
                column: "HangHoaThiTruongId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaChiTiets_ThuThapGiaThiTruongId_HangHoaThiTruongId",
                table: "ThuThapGiaChiTiets",
                columns: new[] { "ThuThapGiaThiTruongId", "HangHoaThiTruongId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_LoaiGiaId",
                table: "ThuThapGiaThiTruongs",
                column: "LoaiGiaId");

            migrationBuilder.CreateIndex(
                name: "IX_ThuThapGiaThiTruongs_NhomHangHoaId",
                table: "ThuThapGiaThiTruongs",
                column: "NhomHangHoaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HangHoa");

            migrationBuilder.DropTable(
                name: "ThuThapGiaChiTiets");

            migrationBuilder.DropTable(
                name: "NhomHangHoa");

            migrationBuilder.DropTable(
                name: "ThuThapGiaThiTruongs");

            migrationBuilder.DropTable(
                name: "Dm_HangHoaThiTruong");

            migrationBuilder.DropTable(
                name: "LoaiGias");

            migrationBuilder.DropTable(
                name: "Dm_ThuocTinh");

            migrationBuilder.DropTable(
                name: "DonViTinh");
        }
    }
}
