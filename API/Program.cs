using Application.Services;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using API.Middlewares;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using Application.MediatorUserHandler;
using Domain.UserEventsHandler;
using MediatR.Pipeline;
using Application.PreAndPostProcess;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? string.Empty;
var expiryMinutes = double.Parse((jwtSettings["ExpiryMinutes"]) ?? "0");
var issuer = jwtSettings["Issuer"] ?? string.Empty;
var audience = jwtSettings["Audience"] ?? string.Empty;

builder.Services.AddSingleton<string>(sp => builder.Configuration["Jwt:SecretKey"]!);
builder.Services.AddSingleton<string>(sp => builder.Configuration["Jwt:ExpiryMinutes"]!);
builder.Services.AddSingleton<string>(sp => builder.Configuration["Jwt:Issuer"]!);
builder.Services.AddSingleton<string>(sp => builder.Configuration["Jwt:Audience"]!);

builder.Services.AddDbContext<TimeSheetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<GeneratePdfService>();
builder.Services.AddScoped<GenerateExcelService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<ITimesheetRepository, TimesheetRepository>();
builder.Services.AddScoped<TimesheetService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>(provider =>
    new UserService(
        provider.GetRequiredService<IUnitOfWork>(),
        secretKey,
        expiryMinutes,
        issuer,
        audience
    ));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateUserHandler).Assembly,
    typeof(GetUserHandler).Assembly,
    typeof(UpdateUserHandler).Assembly,
    typeof(DeleteUserHandler).Assembly,
    typeof(AuthenticateUserHandler).Assembly,
    typeof(UserCreatedEventHandler).Assembly
));
builder.Services.AddTransient(typeof(IRequestPreProcessor<>), typeof(UserCreatePreProcessor<>));
builder.Services.AddTransient(typeof(IRequestPostProcessor<,>), typeof(UserCreatePostProcessor<,>));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseErrorHandlingMiddleware();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program();