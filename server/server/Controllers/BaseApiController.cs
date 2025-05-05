using Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
using server.Helpers;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        public readonly static string THONGBAO = "Thông báo";

        /// <summary>
        /// Executes an async operation that returns a paged list and applies pagination header
        /// </summary>
        protected async Task<ActionResult<PagedList<T>>> ExecutePagedAsync<T>(
            Func<Task<PagedList<T>>> operation)
        {
            var result = await operation();
            Response.AddPaginationHeader(result);
            return Ok(result);
        }

        /// <summary>
        /// Executes an operation that requires existence check and returns NoContent
        /// </summary>
        protected async Task<ActionResult<ApiResponse<Guid>>> ExecuteWithExistenceCheckAsync(
          Guid id,
          Func<Task<bool>> existsCheck,
          Func<Task<bool>> operation,
          string? notFoundMessage = null,
          string? successMessage = null,
          string? failureMessage = null)
        {
            if (!await existsCheck())
            {
                return NotFound(ApiResponse<Guid>.NotFound(message: notFoundMessage));
            }

            if (await operation())
            {
                return Ok(ApiResponse<Guid>.Success(
                    data: id,
                    title: THONGBAO,
                    message: successMessage));
            }

            return BadRequest(ApiResponse<Guid>.BadRequest(message: failureMessage));
        }
    }
}
