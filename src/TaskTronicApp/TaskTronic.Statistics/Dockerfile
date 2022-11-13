#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaskTronic.Statistics/TaskTronic.Statistics.csproj", "TaskTronic.Statistics/"]
COPY ["TaskTronic/TaskTronic.csproj", "TaskTronic/"]
RUN dotnet restore "TaskTronic.Statistics/TaskTronic.Statistics.csproj"
COPY . .
WORKDIR "/src/TaskTronic.Statistics"
RUN dotnet build "TaskTronic.Statistics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskTronic.Statistics.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskTronic.Statistics.dll"]