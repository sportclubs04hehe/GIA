using server.Extensions;
using server.Middleware;
using System.Text;

namespace server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);

            // Add services to the container.
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Development-specific middleware can be added here if needed
            }
            app.UseRouting();

            //app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
