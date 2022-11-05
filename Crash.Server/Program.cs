using Crash.Server;
using Crash.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

var databaseFile = "Databse.db";
builder.Services.AddSignalR();

builder.Services.AddDbContext<CrashContext>(options =>
               options.UseSqlite($"Data Source={databaseFile}"));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHub<CrashHub>("/Crash");

app.Run();