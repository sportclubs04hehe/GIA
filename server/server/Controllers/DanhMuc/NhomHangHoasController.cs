using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.ServiceInterface.IDanhMuc;
using Core.Helpers;
using server.Helpers;
using Application.DTOs.DanhMuc.NhomHangHoasDto;
using Application.ServiceImplement.DanhMuc;
using server.Errors;

namespace server.Controllers.DanhMuc
{ 
    public class NhomHangHoasController : BaseApiController
    {
        private readonly INhomHangHoaService _nhomHangHoaService;

        public NhomHangHoasController(INhomHangHoaService nhomHangHoaService)
        {
            _nhomHangHoaService = nhomHangHoaService;
        }

       
    }
}
