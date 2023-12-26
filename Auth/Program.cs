using Auth.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
// adding authenctication handler and adding the cookie authentication handler 
builder.Services.AddAuthentication().AddCookie("MyCookie",options =>
{
    //configure option, specifing cookie name 
    options.Cookie.Name = "MyCookie";
    options.LoginPath = "/Account/Login"; // explicitly providing the login path
    options.AccessDeniedPath = "/Account/AccessDenied"; // Added explictly path for access denied page
    options.ExpireTimeSpan = TimeSpan.FromSeconds(60);  // Added a cookie expire time
});
// Adding Authorization
builder.Services.AddAuthorization(options =>
{
    // Adding the 1st policy MustBelongToHRDepartment to access HR page
    options.AddPolicy("MustBelongToHRDepartment",
        policy => policy.RequireClaim("Department", "HR")); //Added this Department claim with HR value to our policy MustBelongToHRDepartment,
                                                            //means this policy will require a claim Department with value HR then only it will Authorize 
    options.AddPolicy("AdminOnly",
        policy => policy.RequireClaim("Admin")); //Added a new policy AdminOnly with Admin Claim to access Setting page 

    options.AddPolicy("HRManagerOnly",   // Added a new policy 
        policy => policy
        .RequireClaim("Department", "HR")
        .RequireClaim("Manager")
        .Requirements.Add(new HRManagerProbationRequirement(3))  // added our special claim handler 
        );
});

//This will call our web api automatically
builder.Services.AddHttpClient("OurWebApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7062/");
});

//Configuring session MW to store our JWT 
builder.Services.AddSession(session =>
{
    session.Cookie.HttpOnly = true; //secure cookie 
    session.IdleTimeout = TimeSpan.FromMinutes(20); //session expires in 20 minutes
    session.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>(); // DI our service
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days.
    // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
