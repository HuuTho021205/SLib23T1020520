using SLib23T1020520.BusinessLayer;
using SLib23T1020520.DataLayer.Interfaces;
using SLib23T1020520.DataLayer.SQLServer;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký DbHelper
builder.Services.AddSingleton<DbHelper>();

// ĐĂNG KÝ REPOSITORIES (Thiếu trong code cũ)
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IMemberTypeRepository, MemberTypeRepository>();

// ĐĂNG KÝ SERVICES (Đã xóa 1 dòng AuthorService bị trùng lặp)
builder.Services.AddScoped<AuthorService>();
builder.Services.AddScoped<PublisherService>();
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<MemberTypeService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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