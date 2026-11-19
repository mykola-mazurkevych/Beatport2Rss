FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]
COPY ["web/src/Beatport2Rss.Web/Beatport2Rss.Web.csproj", "web/src/Beatport2Rss.Web/"]

RUN dotnet restore "web/src/Beatport2Rss.Web/Beatport2Rss.Web.csproj"

COPY ["web/src/Beatport2Rss.Web/", "web/src/Beatport2Rss.Web/"]

RUN dotnet publish "web/src/Beatport2Rss.Web/Beatport2Rss.Web.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f --max-time 2 http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Beatport2Rss.Web.dll"]
