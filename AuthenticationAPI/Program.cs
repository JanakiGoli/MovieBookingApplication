using AccessIdentityAPI.Data;
using AccessIdentityAPI.Interfaces;
using AccessIdentityAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<ITokenService,TokenService>();
            //Configures identity services to use MongoDB as storage and adds
            //default token providers.

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(builder.Configuration.GetValue<string>("MongoDbConfig:ConnectionString"), builder.Configuration.GetValue<string>("MongoDbConfig:DatabaseName"))
                .AddDefaultTokenProviders();
           
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            builder.Services.AddScoped<UserManager<ApplicationUser>>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
        );
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                InsertData.Initialize(services);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            } 
            app.UseHttpsRedirection();
            app.UseCors(builder=>builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}