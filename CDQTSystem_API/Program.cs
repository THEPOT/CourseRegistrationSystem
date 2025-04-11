


using Microsoft.EntityFrameworkCore;
using CDQTSystem_API.Constants;
using CDQTSystem_API.Authorization;
using System.Text.Json.Serialization;
using CDQTSystem_API.Converter;
using CDQTSystem_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using CDQTSystem_API.Middlewares;



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
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(CorsConstant.PolicyCreatingOrderWithoutAuthentication, policy =>
					policy.Requirements.Add(new HeaderRequirement("ngobachuyen", "soramyo")));
			});
			builder.Services.AddControllers().AddJsonOptions(x =>
			{
				x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				x.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
			});

			builder.Services.AddDatabase();
			builder.Services.AddSingleton<IAuthorizationHandler, HeaderRequirementHandler>();

			builder.Services.AddUnitOfWork();
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddServices(builder.Configuration);
			builder.Services.AddJwtValidation();
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			builder.Services.AddAutoMapperConfig(builder.Configuration);
			builder.Services.AddConfigSwagger();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddHttpClient();



			var app = builder.Build();


			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseMiddleware<ExceptionHandlingMiddleware>();


			app.UseCors(CorsConstant.PolicyName);
			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
