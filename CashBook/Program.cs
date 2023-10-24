using Microsoft.EntityFrameworkCore;
using DataAccessLayer;
using DataAccessLayer.Abstraction;
using DataAccessLayer.Repositories;
using Logger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddScoped<IDbContext, AppDb>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddControllersWithViews();
builder.Services.AddMvc(); //ViewComponent

builder.Services.AddDbContext<AppDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext")
    ?? throw new InvalidOperationException("Connection string 'DBConnectionString' not found.")));

CashBookLoggerSettings loggerSettings = new CashBookLoggerSettings();
builder.Configuration.GetSection("LoggerSettings").Bind(loggerSettings);
Logger.CashBookLogger.Create(loggerSettings);

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
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
