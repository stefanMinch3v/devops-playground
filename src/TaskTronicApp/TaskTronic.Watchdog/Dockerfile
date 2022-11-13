#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaskTronic.Watchdog/TaskTronic.Watchdog.csproj", "TaskTronic.Watchdog/"]
RUN dotnet restore "TaskTronic.Watchdog/TaskTronic.Watchdog.csproj"
COPY . .
WORKDIR "/src/TaskTronic.Watchdog"
RUN dotnet build "TaskTronic.Watchdog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskTronic.Watchdog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTronic.Watchdog.dll"]