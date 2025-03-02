using CourseRegistration_API.Services.Implements;
using CourseRegistration_API.Services.Interface;
using CourseRegistration_API.Utils;
using CourseRegistration_Domain.Entities;
using CourseRegistration_Domain.Paginate;
using CourseRegistration_Repository.Implement;
using CourseRegistration_Repository.Interfaces;
using Google.Apis.Storage.v1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace CourseRegistration_API.Extensions
{
	public static class DependencyServices
	{
		public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork<UniversityDbContext>, UnitOfWork<UniversityDbContext>>();
			services.AddScoped<IUnitOfWork<DbContext>, UnitOfWork<DbContext>>();

			return services;
		}

		public static IServiceCollection AddDatabase(this IServiceCollection services)
		{
			IConfiguration configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

			services.AddDbContext<UniversityDbContext>(options =>
			options.UseSqlServer(CreateConnectionString(configuration)));

			services.AddScoped<DbContext>(provider => provider.GetService<UniversityDbContext>());
			return services;
		}

		private static string CreateConnectionString(IConfiguration configuration)
		{
			var connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
			return connectionString;
		}

		public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
		{
			// string firebaseCred = config.GetValue<string>("Authentication:FirebaseKey");
			// // string firebaseCred = config.GetValue<string>("AIzaSyCFJOGAnHOQaWntVhN1a16QINIAjVpWaXI");
			// FirebaseApp.Create(new AppOptions()
			// {
			//     Credential = GoogleCredential.FromJson(firebaseCred)
			// }, "[DEFAULT]");

			services.AddScoped<IUsersService, UsersService>();
			services.AddScoped<IStudentsService, StudentsService>();

			return services;
		}

		public static IServiceCollection AddJwtValidation(this IServiceCollection services)
		{
			// Cấu hình JwtUtil từ cấu hình hệ thống (configuration)
			services.AddSingleton<CourseRegistration_API.Utils.JwtUtil>(sp =>
			{
				var configuration = sp.GetRequiredService<IConfiguration>();
				JwtUtil.Initialize(configuration);
				return null;
			});


			var secretKey = "YourStrongSecretKeyThatIsAtLeast32CharactersLong"; 
			var issuer = "CourseRegistrationSystem";
			var audience = "CourseRegistrationClients";

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidIssuer = issuer,
					ValidateIssuer = true,
					ValidAudience = audience,
					ValidateAudience = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
					ClockSkew = TimeSpan.Zero // (tuỳ chọn) giảm độ lệch thời gian khi xác thực token
				};
			});

			return services;
		}


		public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo()
				{
					Title = "University",
					Version = "v1"
				});
				options.MapType<TimeOnly>(() => new OpenApiSchema
				{
					Type = "string",
					Format = "time",
					Example = OpenApiAnyFactory.CreateFromJson("\"13:45:42.0000000\"")
				});
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					In = ParameterLocation.Header,
					Description = "Please enter a valid token",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					BearerFormat = "JWT",
					Scheme = "Bearer"
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						}
					},
					new string[] { }
				}
			});
			});
			return services;
		}
		public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services, IConfiguration config)
		{
			services.AddAutoMapper(typeof(PaginateMapper));
			return services;
		}

	}
}
