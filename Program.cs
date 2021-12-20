using ChatTopics;
using ChatTopics.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ChatTopics";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.LoginPath = "/";
        options.EventsType = typeof(CookieEvents);
    });
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
});
builder.Services.AddSession();
//builder.Services.AddScoped<ChatDB>();
builder.Services.AddSingleton<ChatDB>();
builder.Services.AddScoped<CookieEvents>();
builder.Services.AddHostedService<DeleteJob>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatTopicsHub>("/chat");
    endpoints.MapDefaultControllerRoute();
});

app.Run();