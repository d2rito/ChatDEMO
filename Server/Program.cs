using BlazorApp1.Server.Data;
using BlazorApp1.Server.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

//content root path
env.ContentRootPath = AppContext.BaseDirectory;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


//signal R
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(options =>
{
    options.MimeTypes = ResponseCompressionDefaults
                        .MimeTypes
                        .Concat(new[] { "application/octet-stream" });
});

//swagger
builder.Services.AddSwaggerGen();
//db & identity & auth
builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtIssuer"],
                ValidAudience = builder.Configuration["JwtAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecurityKey"]!))
            };
        });
builder.Services.AddHttpClient();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseRouting();

//always authentication first
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();

app.MapFallbackToFile("index.html");
app.MapHub<ChatHub>("/chathub");
app.Run();
