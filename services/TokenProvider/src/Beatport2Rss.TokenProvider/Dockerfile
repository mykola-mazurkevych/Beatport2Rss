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

# Update package lists first
RUN apt-get update

# Install PuppeteerSharp dependencies and curl for health checks
RUN apt-get install -y \
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
    libxcomposite1 \
    libxcursor1 \
    libxi6 \
    libxtst6 \
    fonts-liberation \
    ca-certificates \
    curl \
    --no-install-recommends && \
    rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Expose gRPC port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_HTTP_PORT=5000
ENV ASPNETCORE_URLS=http://+:5000

# Health check using HTTP health endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

ENTRYPOINT ["dotnet", "Beatport2Rss.TokenProvider.dll"]
