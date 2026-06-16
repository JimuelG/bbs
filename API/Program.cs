using API.Extensions;
using API.Hubs;
using API.Services;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.WebHost.UseWebRoot("wwwroot");
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("https://ugnaybarangay.com", "http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<ITtsService, GoogleTtsService>();
builder.Services.AddScoped<ICertificatePdfService, CertificatePdfService>();
builder.Services.AddSingleton<IOtpService, OtpService>();
builder.Services.Configure<SemaphoreOptions>(
    builder.Configuration.GetSection(SemaphoreOptions.SectionName));
builder.Services.AddHttpClient<ISmsService, SemaphoreSmsService>();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<FailoverService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Required for SameSite.None
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7); // Keeps user logged in for a week
    options.SlidingExpiration = true; 
});

Environment.SetEnvironmentVariable(
    "GOOGLE_APPLICATION_CREDENTIALS",
    Path.Combine(Directory.GetCurrentDirectory(), "project-73902aa4-ac75-493d-b36-72b44044258a.json")
);


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    
}

app.UseCors("CorsPolicy");

app.UseCustomStaticFiles(app.Environment);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();

app.MapControllers();

app.MapGroup("api").MapIdentityApi<AppUser>();
app.MapHub<VerificationHub>("/api/hubs/verification");
app.MapFallbackToController("Index", "Fallback");


try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await context.Database.MigrateAsync();

    await AppContextSeed.SeedAsync(context, userManager, roleManager);

}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
