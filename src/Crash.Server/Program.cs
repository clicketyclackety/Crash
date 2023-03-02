using Crash.Server;
using Crash.Server.Model;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var argHandler = new ArgumentHandler();

// ENSURE THAT FILE CAN BE CREATED
argHandler.EnsureDefaults();
argHandler.ParseArgs(args);

builder.Services.AddSignalR();

// TODO : Where is this DB being put? I think this may be the cause of the issues!
// DB WAS NOT REGENERATED // HOW TO TEST THIS? NOOOOOOOOOO
builder.Services.AddDbContext<CrashContext>(options =>
			   options.UseSqlite($"Data Source={argHandler.DatabaseFileName}"));

// Do we need this?
// builder.WebHost.UseUrls(argHandler.URL);

var app = builder.Build();

// TODO : Make a nice little webpage
app.MapGet("/", () => "Welcome to Crash!");
app.MapHub<CrashHub>("/Crash");

app.MigrateDatabase<CrashContext>();
app.Run();
// Tell Client we're ready!
