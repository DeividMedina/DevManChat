using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Shared.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;
using Web.Controllers;
using Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Web")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<JwtService>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ChatRoomsController>();
builder.Services.AddHttpClient();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            var jwtService = context.HttpContext.RequestServices.GetRequiredService<JwtService>();
            var principal = context.HttpContext.User;
            var jwt = jwtService.GenerateJWT(principal);

            context.Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
                // Add other cookie options as needed
            });

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddCookie()
  .AddJwtBearer();

builder.Services.AddDefaultIdentity<IdentityUser>(options => {

    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6; // Set minimum length

}).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.None, // Adjust this based on your security requirements
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    new { controller = "Home", action = "Index" });
app.MapRazorPages();
app.MapHub<ChatHub>("/chat");
app.Run();
