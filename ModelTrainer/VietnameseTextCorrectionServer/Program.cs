using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VietnameseTextCorrectionServer.Interfaces;
using VietnameseTextCorrectionServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Đăng ký dịch vụ kiểm tra chính tả
builder.Services.AddSingleton<ITextProcessingService, AiTextProcessingService>();

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
