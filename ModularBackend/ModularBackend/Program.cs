using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using ModularBackend.Api.Extensions;
using ModularBackend.Api.Middlewares;
using ModularBackend.Api.Swagger;
using ModularBackend.Application;
using ModularBackend.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var vaultUrl = builder.Configuration["KeyVault:VaultUrl"];

if (!string.IsNullOrWhiteSpace(vaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(vaultUrl),
        new DefaultAzureCredential());
}

// Add services to the container.
builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddCustomRateLimiting(builder.Configuration);
builder.Services.AddCustomCors(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ToldosAPI",
        Version = "v1",
        Description = "Api para gestion de Deco Llavaneres",
        Contact = new OpenApiContact
        {
            Name = "Zakariae Zbakh",
            Email = "zzbakh444@gmail.com"
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    //options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    //{
    //    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    //});
    options.OperationFilter<AuthorizationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CorsExtensions.FrontendPolicy);

app.UseMiddleware<ExceptionsMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapControllers();

app.Run();
