using Crash.Server;
using Crash.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);
var argHandler = new ArgumentHandler();
argHandler.ParseArgs(args);
argHandler.EnsureDefaults();

builder.Services.AddSignalR();

builder.Services.AddDbContext<CrashContext>(options =>
               options.UseSqlite($"Data Source={argHandler.DatabaseFileName}"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapHub<CrashHub>("/Crash");
app.MigrateDatabase<CrashContext>();
app.Run();
// Tell Client we're ready!