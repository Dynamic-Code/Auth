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
    
    options.AddPolicy("HRManagerOnly",
        policy => policy.RequireClaim("Department", "HR").RequireClaim("Manager")); // Added a new policy 
});
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

app.MapRazorPages();

app.Run();
