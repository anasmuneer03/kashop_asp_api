using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Migrations;

namespace KASHOP.PL.Middleware
{
    public class GlobalExceptionHandling
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) {
            try
            {
                await _next(context);
            }
            catch (Exception ex) 
            {
                var errorDetails = new ErrorDetailsResponse()
                {
                    statusCode = StatusCodes.Status500InternalServerError,
                    message = "Server Error ....",
                    innerError = ex.InnerException.Message
                };
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(errorDetails);

            }
        }
    }
}
