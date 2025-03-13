using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;
using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();  // Cấu hình Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7029/"); // Địa chỉ API của bạn
    client.Timeout = TimeSpan.FromSeconds(60);
});
// Đăng ký IHttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true; // Cookie chỉ có thể được truy cập bởi HTTP
    options.Cookie.IsEssential = true; // Cookie là thiết yếu
});

// Cấu hình TempData sử dụng Session
builder.Services.AddRazorPages().AddSessionStateTempDataProvider();

builder.Services.AddAutoMapper(typeof(Program));


// Authentication và Authorization...
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn tới trang đăng nhập nếu chưa đăng nhập
        options.LogoutPath = "/Account/Logout"; // Đường dẫn tới trang đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn nếu không có quyền truy cập
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Đảm bảo session được sử dụng trước authentication
app.UseSession();
app.Use(async (context, next) =>
{
    Console.WriteLine("Session is available: " + context.Session.IsAvailable);
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
