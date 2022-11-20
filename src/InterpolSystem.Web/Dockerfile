#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["InterpolSystem.Web/InterpolSystem.Web.csproj", "InterpolSystem.Web/"]
COPY ["InterpolSystem.Common/InterpolSystem.Common.csproj", "InterpolSystem.Common/"]
COPY ["InterpolSystem.Data/InterpolSystem.Data.csproj", "InterpolSystem.Data/"]
COPY ["InterpolSystem.Services/InterpolSystem.Services.csproj", "InterpolSystem.Services/"]
RUN dotnet restore "InterpolSystem.Web/InterpolSystem.Web.csproj"
COPY . .
WORKDIR "/src/InterpolSystem.Web"
RUN dotnet build "InterpolSystem.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InterpolSystem.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InterpolSystem.Web.dll"]