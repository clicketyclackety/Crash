using Crash.Server;
using Crash.Server.Model;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var argHandler = new ArgumentHandler();
argHandler.EnsureDefaults();
argHandler.ParseArgs(args);

builder.Services.AddSignalR();

builder.Services.AddDbContext<CrashContext>(options =>
			   options.UseSqlite($"Data Source={argHandler.DatabaseFileName}"));

builder.WebHost.UseUrls(argHandler.URL);

var app = builder.Build();

// TODO : Make a nice little webpage
app.MapGet("/", () => "Welcome to Crash!");
app.MapHub<CrashHub>("/Crash");

app.MigrateDatabase<CrashContext>();
app.Run();
// Tell Client we're ready!
