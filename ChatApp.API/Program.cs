
using ChatApp.Repository;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ChatApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // configure database connection
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), x =>
                        x.MigrationsAssembly(Assembly.GetAssembly(typeof(ApplicationDbContext)).GetName().Name)
                    )
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
