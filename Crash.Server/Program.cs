using Crash.Server;
using Crash.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

const string dbName = "Database.db";
const string appName = "Crash";
const string dbDirectory = "App_Data";

// TODO : Ensure this works on OSX
string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
string databasePath = Path.Combine(appData, appName, dbDirectory);
string databaseFile = Path.Combine(databasePath, dbName);

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


