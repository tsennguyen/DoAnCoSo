using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Models.Momo;
using Shopping_Laptop.Repository;
using Shopping_Laptop.Services.Momo;

var builder = WebApplication.CreateBuilder(args);

// kê?t nô?i MOMO API
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
builder.Services.AddScoped<IMomoService, MomoService>();

//confi Login Google account
builder.Services.AddAuthentication(options =>
{
/* options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
   options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
   options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;*/

}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});

// Thêm DbContext
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Shopping_LaptopContext")));

//Add Email  Sender
builder.Services.AddTransient<IEmailSender, EmailSender>();
// C?u hình Identity
builder.Services.AddIdentity<AppUserModel, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>() // ??i t? DbContext -> DataContext
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    // C?u hình m?t kh?u
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // C?u hình user
    options.User.RequireUniqueEmail = true;
});


var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); //dang nhap
app.UseAuthorization(); // kiem tra quyen

// ??nh tuy?n

app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
   name: "category",
   pattern: "category/{slug?}",
   defaults: new { controller = "Category", action = "Index" });

app.MapControllerRoute(
   name: "brand",
   pattern: "brand/{slug?}",
   defaults: new { controller = "Brand", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Product}/{action=Index}/{id?}");
});



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

}

app.Run();