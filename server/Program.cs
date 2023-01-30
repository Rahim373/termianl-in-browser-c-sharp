using Terminal.Web;
using Terminal.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddSignalR();
builder.Services.AddSingleton<ProcessCollection>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:9000")
            //.AllowAnyOrigin()
            .AllowAnyHeader()
            .WithMethods("GET", "POST")
            .AllowCredentials();
    });
});

var app = builder.Build();
app.UseCors("ClientPermission");
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<CommandHub>("/cmdhub");
    endpoints.MapHealthChecks("/health");
});

app.Run();
