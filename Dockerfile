#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY ["src/Crash.Server/Crash.Server.csproj", "src/Crash.Server/"]
COPY ["src/SpeckLib/SpeckLib.csproj", "src/SpeckLib/"]
RUN dotnet restore "src/Crash.Server/Crash.Server.csproj"
COPY . .
WORKDIR "/src/Crash.Server"
RUN dotnet build "Crash.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crash.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Crash.Server.dll"]