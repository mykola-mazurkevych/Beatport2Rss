# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy solution and configuration files
COPY ["Beatport2Rss.slnx", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]

# Copy the Api project and its dependencies
COPY ["api/src/Beatport2Rss.Api/Beatport2Rss.Api.csproj", "api/src/Beatport2Rss.Api/"]
COPY ["api/src/Beatport2Rss.Api.Application/Beatport2Rss.Api.Application.csproj", "api/src/Beatport2Rss.Api.Application/"]
COPY ["api/src/Beatport2Rss.Api.Domain/Beatport2Rss.Api.Domain.csproj", "api/src/Beatport2Rss.Api.Domain/"]
COPY ["api/src/Beatport2Rss.Api.Infrastructure/Beatport2Rss.Api.Infrastructure.csproj", "api/src/Beatport2Rss.Api.Infrastructure/"]
COPY ["api/src/Beatport2Rss.Api.Jobs/Beatport2Rss.Api.Jobs.csproj", "api/src/Beatport2Rss.Api.Jobs/"]
COPY ["common/src/Beatport2Rss.Common.Beatport/Beatport2Rss.Common.Beatport.csproj", "common/src/Beatport2Rss.Common.Beatport/"]
COPY ["common/src/Beatport2Rss.Common.BeatportTokenProvider/Beatport2Rss.Common.BeatportTokenProvider.csproj", "common/src/Beatport2Rss.Common.BeatportTokenProvider/"]
COPY ["common/src/Beatport2Rss.Common.EntityFrameworkCore/Beatport2Rss.Common.EntityFrameworkCore.csproj", "common/src/Beatport2Rss.Common.EntityFrameworkCore/"]
COPY ["common/src/Beatport2Rss.Common.IntegrationEvents/Beatport2Rss.Common.IntegrationEvents.csproj", "common/src/Beatport2Rss.Common.IntegrationEvents/"]
COPY ["common/src/Beatport2Rss.Common.Messaging/Beatport2Rss.Common.Messaging.csproj", "common/src/Beatport2Rss.Common.Messaging/"]
COPY ["common/src/Beatport2Rss.Common.Miscellaneous/Beatport2Rss.Common.Miscellaneous.csproj", "common/src/Beatport2Rss.Common.Miscellaneous/"]
COPY ["common/src/Beatport2Rss.Common.SharedKernel/Beatport2Rss.Common.SharedKernel.csproj", "common/src/Beatport2Rss.Common.SharedKernel/"]
COPY ["tools/Beatport2Rss.SourceGenerator/Beatport2Rss.SourceGenerator.csproj", "tools/Beatport2Rss.SourceGenerator/"]
COPY ["common/protos/", "common/protos/"]

# Restore dependencies
RUN dotnet restore "api/src/Beatport2Rss.Api/Beatport2Rss.Api.csproj"

# Copy source code
COPY ["api/src/", "api/src/"]
COPY ["common/src/", "common/src/"]
COPY ["tools/Beatport2Rss.SourceGenerator/", "tools/Beatport2Rss.SourceGenerator/"]

# Build the project
RUN dotnet build "api/src/Beatport2Rss.Api/Beatport2Rss.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "api/src/Beatport2Rss.Api/Beatport2Rss.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

EXPOSE 8080

ENV ASPNETCORE_HTTP_PORTS=8080

HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Beatport2Rss.Api.dll"]
