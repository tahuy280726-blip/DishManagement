using DishManagement;
using DishManagement.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Việt hóa / Anh hóa các thông báo lỗi model binding (vd: nhập chữ vào ô số)
    var p = options.ModelBindingMessageProvider;
    p.SetValueMustNotBeNullAccessor(_ => L.T("val.required"));
    p.SetMissingBindRequiredValueAccessor(_ => L.T("val.required"));
    p.SetMissingKeyOrValueAccessor(() => L.T("val.required"));
    p.SetAttemptedValueIsInvalidAccessor((_, __) => L.T("val.invalidValue"));
    p.SetNonPropertyAttemptedValueIsInvalidAccessor(_ => L.T("val.invalidValue"));
    p.SetUnknownValueIsInvalidAccessor(_ => L.T("val.invalidValue"));
    p.SetNonPropertyUnknownValueIsInvalidAccessor(() => L.T("val.invalidValue"));
    p.SetValueIsInvalidAccessor(_ => L.T("val.invalidValue"));
    p.SetValueMustBeANumberAccessor(_ => L.T("val.number"));
    p.SetNonPropertyValueMustBeANumberAccessor(() => L.T("val.number"));
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Luôn dùng định dạng số kiểu en-US (dấu chấm thập phân) để không lỗi trên máy Windows tiếng Việt
app.UseRequestLocalization(new Microsoft.AspNetCore.Builder.RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures("en-US")
    .AddSupportedUICultures("en-US"));

// Chọn ngôn ngữ giao diện theo cookie "lang". Lần đầu truy cập (chưa có cookie) => English.
app.Use(async (context, next) =>
{
    L.Lang = context.Request.Cookies["lang"];
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStatusCodePagesWithReExecute("/Home/HttpStatus", "?code={0}");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
