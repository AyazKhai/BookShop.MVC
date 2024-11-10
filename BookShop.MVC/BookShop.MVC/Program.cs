using BookShop.MVC.Data;
using BookShop.MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)); //  PostgreSQL

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ShopUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true;
        //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+�����������娸����������������������������������������������������";
    }).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();



builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



builder.Services.Configure<IdentityOptions>(options =>
{
    // ��������� ������.
    //options.Password.RequireDigit = true; // �������, ����� ������ �������� ���� �� ���� �����.
   // options.Password.RequireLowercase = true; // �������, ����� ������ �������� ���� �� ���� �������� �����.
   // options.Password.RequireNonAlphanumeric = true; // �������, ����� ������ �������� ���� �� ���� ������������ ������ (��������, !, @, # � �. �.).
    //options.Password.RequireUppercase = true; // �������, ����� ������ �������� ���� �� ���� ��������� �����.
    //options.Password.RequiredLength = 6; // ����������� ����� ������ (6 ��������).
    //options.Password.RequiredUniqueChars = 1; // �������, ����� � ������ ���� �� ����� ������ ����������� �������.


    // ��������� ���������� ������� ������.
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // ����� ���������� ������� ������ ����� ���������� ������������� ���������� ��������� ������� ����� (5 �����).
    //options.Lockout.MaxFailedAccessAttempts = 5; // ������������ ���������� ��������� ������� �����, ����� ������� ������� ������ ����� �������������.
    //options.Lockout.AllowedForNewUsers = true; // ��������� ���������� ������� ������� ��� ����� �������������.

    // User settings.
    //options.User.AllowedUserNameCharacters =
    //"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+�����������娸����������������������������������������������������";
   
    options.User.RequireUniqueEmail = false;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Add Razor Pages services

builder.Services.AddHttpClient(name: "BookShop.WebAPI",
    configureClient: options =>
    {
        options.BaseAddress = new Uri("https://localhost:7299/");
        options.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json", 1.0));
    });
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

//for cors

app.UseAuthentication(); // Ensure Authentication is included
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
