using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DataAccess.Contexts;
using BusinessObject.Models;
using System.Text;
using AutoMapper;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<AspNetUsers, AspNetRoles>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Lấy khóa JWT từ cấu hình
var jwtKey = builder.Configuration["JwtSettings:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new ArgumentNullException("JwtSettings:Key", "JWT Key is missing in configuration.");
}

// Cấu hình Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });


// Thêm Authorization
builder.Services.AddAuthorization();

// Cấu hình HttpClient
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7029/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Cấu hình Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

// Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()    // Cho phép bất kỳ nguồn (origin) nào
               .AllowAnyMethod()    // Cho phép bất kỳ phương thức HTTP nào (GET, POST, PUT, DELETE, ...)
               .AllowAnyHeader();   // Cho phép bất kỳ header nào trong yêu cầu
    });
});

// Thêm Razor Pages và Controllers
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Thêm log để xác nhận
app.Logger.LogInformation("Application starting with Cookies authentication configured.");

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseCors("AllowAll");
app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapControllers();

// Seed admin account
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AspNetUsers>>();
    var roleManager = services.GetRequiredService<RoleManager<AspNetRoles>>();
    await SeedAdminAccount(userManager, roleManager, builder.Configuration);
}

app.Run();

// Hàm SeedAdminAccount
async Task SeedAdminAccount(UserManager<AspNetUsers> userManager, RoleManager<AspNetRoles> roleManager, IConfiguration config)
{
    string adminEmail = config["AdminAccount:Email"]!;
    string adminPassword = config["AdminAccount:Password"]!;
    string adminRole = config["AdminAccount:Role"]!;

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var role = new AspNetRoles
        {
            Name = adminRole,
            NormalizedName = adminRole.ToUpper()
        };
        await roleManager.CreateAsync(role);
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AspNetUsers
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User"
        };
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}