using TestAccura.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System.Net.Mail;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using TestAccura.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
builder.Services.AddHttpClient();



builder.Services.AddDbContext<AccuraContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<SmtpClient>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});


//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//    });

void ConfigureLogging(ILoggingBuilder logging)
{
    logging.AddConsole(); // Add console logging provider
    logging.AddDebug();   // Add debug output logging provider
    // Add other logging providers and configuration here
}


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

// ... other middleware ...


app.UseStaticFiles(); // This line configures static files middleware

app.Use(async (context, next) =>
{
    // Set the web root path
    context.Request.PathBase = "/public"; // Customize this path as needed
    await next();
});


//static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
//   WebHost.CreateDefaultBuilder(args)
//       .UseStartup<Startup>();
//


// Use authentication middleware
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
