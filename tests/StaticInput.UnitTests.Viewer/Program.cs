using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using StaticInput.UnitTests.Viewer.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
builder.Services.AddMudServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>();

app.MapPost("TestRedirect", ([FromForm] string returnUrl) => TypedResults.LocalRedirect($"~/{returnUrl}"));

app.MapPost("TestEndpoint", () => Results.Ok("Success"));

app.Run();
