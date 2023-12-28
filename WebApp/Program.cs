using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

//Adding SqlServer configuration
builder.Services.AddDbContext<ApplicationDBConext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

//DI to configure Identity, IdentityUser, IdentityRole comes from nuget package
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => // configure identity
{
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true; // this make the email confirmation manditory
})

    // tell identity to use this ApplicationDBConext to connect with actual DB
    .AddEntityFrameworkStores<ApplicationDBConext>()
    .AddDefaultTokenProviders(); //token provider for emailconfirmationtoken

builder.Services.ConfigureApplicationCookie(options => // Identity also uses cookie so we need to configure the cookie as well
{
    options.LoginPath = "/Account/login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); //enable authentication
app.UseAuthorization();

app.MapRazorPages();

app.Run();
