#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaskTronic.Drive.Gateway/TaskTronic.Drive.Gateway.csproj", "TaskTronic.Drive.Gateway/"]
COPY ["TaskTronic/TaskTronic.csproj", "TaskTronic/"]
RUN dotnet restore "TaskTronic.Drive.Gateway/TaskTronic.Drive.Gateway.csproj"
COPY . .
WORKDIR "/src/TaskTronic.Drive.Gateway"
RUN dotnet build "TaskTronic.Drive.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskTronic.Drive.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTronic.Drive.Gateway.dll"]