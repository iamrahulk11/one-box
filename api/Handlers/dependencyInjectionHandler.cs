using Asp.Versioning;
using AspNetCoreRateLimit;
using domain.DTOs;
using domain.Interfaces;
using domain.Services;
using domain.Wrapper;
using helpers;
using infrastructure;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace api.Handlers
{
    public static class dependencyInjectionHandler
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();

            services.AddControllers();
            services.AddScoped<sqlWrapper>();
            services.AddScoped<oracleWrapper>();
            services.AddScoped<email>();
            services.AddScoped<restApi>();
            services.AddSingleton<BloomFilterWrapper>();
            services.AddSingleton<BloomFilterPopulator>();

            services.AddScoped<LoginService>();
            
            services.AddScoped<ILogin, LoginRepository>();
            services.AddScoped<IUser, UserRepository>();
            services.AddScoped<ICommon, CommonRepository>();


            return services;
        }

        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {

            //Add Api versioning and explorer 
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new(1, 0);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                new HeaderApiVersionReader("x-api-version"),
                                                                new MediaTypeApiVersionReader("x-api-version"));
            }).AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
                setup.AssumeDefaultVersionWhenUnspecified = true;
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    //Description = "Please enter a valid token",
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
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            //Modifying the data-annotations error format into base response
            services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = (errorContext) =>
                {
                    baseResponseDto<object> responseToSend = new();
                    resultResponseDto _resultResponse = new();
                    var errors = errorContext.ModelState
                                                        .Values
                                                        .SelectMany(e => e.Errors.Select(m => new
                                                        {
                                                            m.ErrorMessage
                                                        })).ToList();

                    foreach (var messages in errors)
                    {
                        _resultResponse.flag_message = messages.ErrorMessage;
                    }

                    _resultResponse.flag = 0;
                    responseToSend.Result = _resultResponse;
                    return new BadRequestObjectResult(responseToSend);
                };
            });
            return services;
        }

        public static IServiceCollection AddAuthenticationAndRateLimit(this IServiceCollection services,
                                                                          IConfiguration configuration)
        {
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));

            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["jwtConfiguration:Audience"],
                    ValidIssuer = configuration["jwtConfiguration:Issuer"],
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwtConfiguration:Secret"]))
                }; 
            });

            return services;
        }
    }
}
