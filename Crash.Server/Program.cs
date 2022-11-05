using Crash.Server;
using Crash.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

var databasePath = "App_Data";
var databaseFile = Path.Combine(databasePath, "Database.db");

if (!Directory.Exists(databasePath))
{
    Directory.CreateDirectory(databasePath);
}

builder.Services.AddSignalR();

builder.Services.AddDbContext<CrashContext>(options =>
               options.UseSqlite($"Data Source={databaseFile.Replace("\\", "/")}"));

var app = builder.Build();


app.MapGet("/", () => "Hello World!");

app.MapHub<CrashHub>("/Crash");

app.MigrateDatabase<CrashContext>();

app.Run();


