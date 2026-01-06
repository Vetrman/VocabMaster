using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using VocabMaster.Data;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ƒŒ¡¿¬»“‹ ÀŒ√»–Œ¬¿Õ»≈ SQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging()  // œŒ ¿«€¬¿≈“ «Õ¿◊≈Õ»ﬂ
           .LogTo(Console.WriteLine, LogLevel.Information));  // ÀŒ√» ¬  ŒÕ—ŒÀ‹

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();