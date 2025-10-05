using api.Handlers;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Load configuration based on the current environment
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
       .AddInfrastructure()
       .AddAuthenticationAndRateLimit(builder.Configuration)
       .AddPresentation();

//Configure Logging from appsetting.json
var logger = new LoggerConfiguration()
                        .ReadFrom
                        .Configuration(builder.Configuration)
                        .Enrich.FromLogContext()
                        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() ||
    app.Environment.IsEnvironment("Local"))
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            if (app.Environment.IsDevelopment())
                options.SwaggerEndpoint($"/api/swagger/{description.GroupName}/swagger.json", description.GroupName);
            else
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
            options.DocumentTitle = "one-box-be"; // Changed the name with the solution/project name.
            //Disables the schema part from swagger UI.
            options.DefaultModelsExpandDepth(-1);
            //options.SupportedSubmitMethods(); // Disables try out feature from the api's.
        }
    });
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Documents")),
    RequestPath = "/Documents"
});

app.UseCors(options => options.WithOrigins("http://localhost:3001")
                                            .AllowAnyMethod()
                                            .AllowAnyHeader());

app.UseIpRateLimiting();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseGlobalException();

app.Run();