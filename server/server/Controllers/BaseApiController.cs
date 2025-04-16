using Core.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Errors;
using server.Helpers;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        
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
        protected async Task<IActionResult> ExecuteWithExistenceCheckAsync(
            Guid id,
            Func<Task<bool>> existsCheck,
            Func<Task<bool>> operation,
            string notFoundMessage = null,
            string successMessage = null,
            string failureMessage = "Operation failed")
        {
            if (!await existsCheck())
                return NotFound(notFoundMessage ?? $"Item with ID {id} not found");
                
            if (await operation())
            {
                LogSuccess(successMessage);
                return NoContent();
            }
            
            return BadRequest(failureMessage);
        }
        
        /// <summary>
        /// Executes a validated update operation
        /// </summary>
        protected async Task<IActionResult> ExecuteValidatedUpdateAsync<T>(
            Guid id,
            T updateDto,
            Func<Task<bool>> existsCheck,
            Func<Task<(bool IsValid, string ErrorMessage)>> validator,
            Func<Task<bool>> updateOperation,
            string notFoundMessage = null,
            string successMessage = null,
            string failureMessage = "Update failed")
        {
            // Existence check
            if (!await existsCheck())
                return NotFound(notFoundMessage ?? $"Item with ID {id} not found");
            
            // Validation
            var validation = await validator();
            if (!validation.IsValid)
                return BadRequest(validation.ErrorMessage);
            
            // Update operation
            if (await updateOperation())
            {
                LogSuccess(successMessage);
                return NoContent();
            }
            
            return BadRequest(failureMessage);
        }
        
        /// <summary>
        /// Helper method to log success messages if provided
        /// </summary>
        private void LogSuccess(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            
            var logger = HttpContext.RequestServices.GetService(typeof(ILogger<BaseApiController>)) as ILogger<BaseApiController>;
            logger?.LogInformation(message);
        }
    }
}
