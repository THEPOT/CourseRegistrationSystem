﻿using CDQTSystem_API.Consumers;
using CDQTSystem_API.Services.Implements;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Utils;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Domain.Paginate;
using CDQTSystem_Repository.Implement;
using CDQTSystem_Repository.Interfaces;
using Google.Apis.Storage.v1;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace CDQTSystem_API.Extensions
{
	public static class DependencyServices
	{
		public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
		{
			services.AddScoped<IUnitOfWork<CdqtsystemContext>, UnitOfWork<CdqtsystemContext>>();
			services.AddScoped<IUnitOfWork<DbContext>, UnitOfWork<DbContext>>();

			return services;
		}

		public static IServiceCollection AddDatabase(this IServiceCollection services)
		{
			IConfiguration configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

			services.AddDbContext<CdqtsystemContext>(options =>
				options.UseSqlServer(CreateConnectionString(configuration),
					sqlServerOptions => sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
			);

			services.AddScoped<DbContext>(provider => provider.GetService<CdqtsystemContext>());
			return services;
		}

		public static IServiceCollection AddMassTransit(this IServiceCollection services)
		{
			services.AddMassTransit(x =>
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
			return services;
		}

		private static string CreateConnectionString(IConfiguration configuration)
		{
			var connectionString = configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
			return connectionString;
		}


		public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<ICourseEvaluationService, CourseEvaluationService>();
			services.AddScoped<ICourseOfferingService, CourseOfferingService>();
			services.AddScoped<ICourseRegistrationService, CourseRegistrationService>();
			services.AddScoped<ICourseService, CourseService>();
			services.AddScoped<IDepartmentService, DepartmentService>();
			services.AddScoped<IDegreeAuditService, DegreeAuditService>();
			services.AddScoped<IEvaluationQuestionService, EvaluationQuestionService>();
			services.AddScoped<IGradesService, GradesService>();
			services.AddScoped<IMajorService, MajorService>();
			services.AddScoped<IMidtermEvaluationService, MidtermEvaluationService>();
			services.AddScoped<IProfessorService, ProfessorService>();
			services.AddScoped<IRegistrationPeriodService, RegistrationPeriodService>();
			services.AddScoped<IScholarshipsService, ScholarshipsService>();
			services.AddScoped<ISemesterService, SemesterService>();
			services.AddScoped<IServiceRequestService, ServiceRequestService>();
			services.AddScoped<IStudentsService, StudentsService>();
			services.AddScoped<ITuitionService, TuitionService>();
			services.AddScoped<IUsersService, UsersService>();

			return services;
		}

		public static IServiceCollection AddJwtValidation(this IServiceCollection services)
		{
			// Cấu hình JwtUtil từ cấu hình hệ thống (configuration)
			services.AddSingleton<JwtUtil>(sp =>
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
					ValidateLifetime = true,
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

	}
}
