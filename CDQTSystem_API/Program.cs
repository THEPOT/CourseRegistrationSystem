


using Microsoft.EntityFrameworkCore;
using CDQTSystem_API.Constants;
using CDQTSystem_API.Authorization;
using System.Text.Json.Serialization;
using CDQTSystem_API.Converter;
using CDQTSystem_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using CDQTSystem_API.Middlewares;
using MassTransit;
using CDQTSystem_API.Consumers;
using CDQTSystem_API.Messages;
using CDQTSystem_Repository.Interfaces;

namespace CourseRegistrationSystem
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Logging.ClearProviders();
			builder.Logging.AddConsole();
			builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
			
			// Database and Repository registrations
			builder.Services.AddDatabase();
			builder.Services.AddUnitOfWork();

			// Add Authorization services
			builder.Services.AddAuthentication();
			builder.Services.AddAuthorization();

			// Other service registrations
			builder.Services.AddCors(options =>
			{
				options.AddPolicy(name: CorsConstant.PolicyName,
					policy =>
					{
						policy
							.WithOrigins("http://localhost:5173")
							.AllowAnyHeader()
							.AllowAnyMethod()
							.AllowCredentials();
					});
			});

			// MassTransit Configuration
			builder.Services.AddMassTransit(x =>
			{
				x.AddConsumer<CourseRegistrationConsumer>();
				
				x.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host("localhost", "/", h =>
					{
						h.Username("guest");
						h.Password("guest");
					});

					cfg.ConfigureEndpoints(context);
					
					cfg.UseMessageRetry(r =>
					{
						r.Intervals(100, 500, 1000);
					});

					cfg.UseTimeout(t =>
					{
						t.Timeout = TimeSpan.FromSeconds(60);
					});
				});
			});

			// Existing service registrations...
			builder.Services.AddSingleton<IAuthorizationHandler, HeaderRequirementHandler>();
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddServices(builder.Configuration);
			builder.Services.AddJwtValidation();
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			builder.Services.AddAutoMapperConfig(builder.Configuration);
			builder.Services.AddConfigSwagger();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddHttpClient();
			builder.Services.AddControllers();

			var app = builder.Build();

			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseMiddleware<ExceptionHandlingMiddleware>();

			app.UseCors(CorsConstant.PolicyName);
			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
