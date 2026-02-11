using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<ITtsService, GoogleTtsService>();
builder.Services.AddAuthentication();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

Environment.SetEnvironmentVariable(
    "GOOGLE_APPLICATION_CREDENTIALS",
    Path.Combine(Directory.GetCurrentDirectory(), "project-73902aa4-ac75-493d-b36-72b44044258a.json")
);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>();

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
