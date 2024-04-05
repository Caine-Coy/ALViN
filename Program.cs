using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ALViN.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
//Add Support for HTTP GET Controller
builder.Services.AddControllers();
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true; // Bypass SSL certificate validation

builder.Services.AddScoped(sp => new HttpClient(handler)
{
    // Set your external API base address here
    BaseAddress = new Uri("https://127.0.0.1:7145/api/")
});
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
