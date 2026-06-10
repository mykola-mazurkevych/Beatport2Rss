# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and configuration files
COPY ["Beatport2Rss.slnx", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]

# Copy the TokenProvider project and its dependencies
COPY ["services/TokenProvider/src/Beatport2Rss.TokenProvider/Beatport2Rss.TokenProvider.csproj", "services/TokenProvider/src/Beatport2Rss.TokenProvider/"]

# Restore dependencies
RUN dotnet restore "services/TokenProvider/src/Beatport2Rss.TokenProvider/Beatport2Rss.TokenProvider.csproj"

# Copy source code
COPY ["services/TokenProvider/src/", "services/TokenProvider/src/"]

# Build the project
RUN dotnet build "services/TokenProvider/src/Beatport2Rss.TokenProvider/Beatport2Rss.TokenProvider.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "services/TokenProvider/src/Beatport2Rss.TokenProvider/Beatport2Rss.TokenProvider.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

ENV DEBIAN_FRONTEND=noninteractive

# Update package lists and install PuppeteerSharp/Chromium dependencies in one layer
RUN apt-get update && apt-get install -y --no-install-recommends \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libxrandr2 \
    libpango-1.0-0 \
    libatk1.0-0 \
    libatk-bridge2.0-0 \
    libcups2 \
    libdbus-1-3 \
    libexpat1 \
    libfontconfig1 \
    libgbm1 \
    libglib2.0-0 \
    libgtk-3-0 \
    libnspr4 \
    libnss3 \
    libpulse0 \
    libasound2t64 \
    libxcomposite1 \
    libxcursor1 \
    libxi6 \
    libxtst6 \
    libxss1 \
    libx11-xcb1 \
    libxdamage1 \
    libxkbcommon0 \
    libxcb1 \
    libxfixes3 \
    libgcc-s1 \
    libatspi2.0-0 \
    libappindicator3-1 \
    fonts-liberation \
    ca-certificates \
    curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Expose gRPC port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_HTTP_PORT=5000

# Health check using plain HTTP for the health endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5001/health || exit 1

ENTRYPOINT ["dotnet", "Beatport2Rss.TokenProvider.dll"]
