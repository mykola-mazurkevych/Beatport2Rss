# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Beatport2Rss.slnx", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
COPY ["common/src/", "common/src/"]
COPY ["services/collector/src/", "services/collector/src/"]

RUN dotnet restore "services/collector/src/Beatport2Rss.Collector.Worker/Beatport2Rss.Collector.Worker.csproj"
RUN dotnet publish "services/collector/src/Beatport2Rss.Collector.Worker/Beatport2Rss.Collector.Worker.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Beatport2Rss.Collector.Worker.dll"]