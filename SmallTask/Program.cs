using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Json;
using SmallTask.Settings;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(cb => cb.RegisterModule<SmallTask.AutofacModule>());

// Connection string: set in appsettings.json under "AppSettings": { "ConnectionString": "..." }
// Or override in appsettings.Development.json. Database is created/updated on first run via Migrate().
var conn = builder.Configuration.GetSection(AppSettings.SectionName)[nameof(AppSettings.ConnectionString)];
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(conn, sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));

builder.Services.AddControllersWithViews();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    o.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
});

builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Only redirect to HTTPS when the host is configured with HTTPS URLs (avoids redirect loop when hosted with only HTTP, e.g. from SmallTaskList.App).
var urls = app.Configuration["urls"] ?? builder.Configuration["urls"] ?? "";
if (urls.IndexOf("https", StringComparison.OrdinalIgnoreCase) >= 0)
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Apply pending migrations on startup (creates DB and tables if they don't exist)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");


app.Run();
