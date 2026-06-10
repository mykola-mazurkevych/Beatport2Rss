# TokenProvider Service

A gRPC microservice that provides Beatport access tokens via an in-memory cache with automatic refresh.

## Overview

The TokenProvider service exposes a single gRPC endpoint that returns Beatport access tokens. It handles:
- **Token fetching** from Beatport using headless Chromium browser automation
- **Caching** with automatic expiration based on token TTL
- **Thread-safe operations** using semaphore-based locking

## Architecture

### Components

- **TokenService** (gRPC): Main service exposing `GetToken()` RPC
- **AccessTokenProvider**: Manages in-memory token cache with expiration
- **BeatportAccessTokenProvider**: Handles token acquisition from Beatport
- **ChromiumDownloader**: Downloads and manages Chromium browser for automation

### Technology Stack

- **.NET 10** with ASP.NET Core
- **gRPC** for service communication
- **PuppeteerSharp** for browser automation
- **Docker** for containerization

## Local Development

### Prerequisites

- Docker and Docker Compose
- .NET 10 SDK (optional, for local development without Docker)
- Beatport credentials

### Setup

1. Copy environment file (from solution root):
   ```bash
   cp docker/.env.example docker/.env
   ```

2. Update `docker/.env` with your credentials:
   ```
   POSTGRES_USER=db@beatport2rss.com
   POSTGRES_PASSWORD=your_secure_password
   POSTGRES_DB=beatport2rss
   BEATPORT_USERNAME=your_username
   BEATPORT_PASSWORD=your_password
   ```

3. Start the services with Docker Compose (from solution root):
   ```bash
   docker-compose -f docker/docker-compose.yml up -d
   ```

The service will be available at `http://localhost:5000`

### Building Locally

```bash
dotnet build services/TokenProvider/src/Beatport2Rss.TokenProvider
dotnet run --project services/TokenProvider/src/Beatport2Rss.TokenProvider
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Production` |
| `BeatportCredentials__Username` | Beatport username | Required |
| `BeatportCredentials__Password` | Beatport password | Required |
| `ChromiumDownloaderOptions__BaseAddress` | Chromium download URL | `https://edgedl.me.chromium.org/chromium-browser-snapshots` |

### appsettings.json

Key configuration:
```json
{
  "Kestrel": {
    "Endpoints": {
      "gRpc": {
        "Url": "http://0.0.0.0:5000",
        "Protocols": "Http2"
      }
    }
  }
}
```

## gRPC API

### GetToken

**Request:**
```protobuf
message Empty {}
```

**Response:**
```protobuf
message TokenReply {
  string access_token = 1;
}
```

**Usage Example (with grpcurl):**
```bash
grpcurl -plaintext localhost:5000 token.TokenService/GetToken
```

## Docker Deployment

### Build Image

```bash
docker build -f services/TokenProvider/docker/Dockerfile -t beatport-token-provider .
```

### Run Container

```bash
docker run -p 5000:5000 \
  -e BeatportCredentials__Username=your_username \
  -e BeatportCredentials__Password=your_password \
  beatport-token-provider
```

### Using Docker Compose

From the solution root:

```bash
# Copy environment template
cp docker/.env.example docker/.env

# Edit with your credentials
# nano docker/.env  (or your preferred editor)

# Start services
docker-compose -f docker/docker-compose.yml up -d

# Stop services
docker-compose -f docker/docker-compose.yml down

# View logs
docker-compose -f docker/docker-compose.yml logs -f
```

## Startup Behavior

- **Chromium Download**: Chromium binary (~200MB) is downloaded on service startup, before the service starts accepting requests
- **Fail-Fast**: If the download fails, the service will not start, providing immediate visibility into deployment issues
- **Subsequent Starts**: On restart, if Chromium is already cached, startup is fast (~2-3 seconds)

## Caching Strategy

- **In-Memory Cache**: Tokens are cached in memory with a TTL matching the token's expiration
- **Thread-Safe**: Uses `SemaphoreSlim` to prevent concurrent token refreshes with double-check locking
- **Automatic Refresh**: Expired tokens are automatically fetched on the next request

## Health Check

The container includes a health check that monitors service availability:

```
GET http://localhost:5000/health
```

## Troubleshooting

### Service fails to start
- Check Beatport credentials are correct and set via environment variables
- Check Chromium download logs: `docker-compose logs token-provider`
- Ensure network connectivity for downloading Chromium
- Verify sufficient disk space for Chromium binary (~200MB)

### Chromium download timeout
- Increase startup timeout in Docker/K8s configuration
- Check network bandwidth and connectivity
- Consider using a local Chromium mirror (configure `ChromiumDownloaderOptions__BaseAddress`)

### Slow first token fetch
- This should not occur - Chromium is downloaded at startup
- If slow, check startup logs for warning messages about Chromium initialization
- Token fetch itself is <100ms when Chromium is ready

### Port conflicts
- Change port mapping in `docker-compose.yml` if port 5000 is in use
- Ensure HTTP/2 support on your network

## Development

### Project Structure

```
services/TokenProvider/
├── src/
│   └── Beatport2Rss.TokenProvider/
│       ├── Services/
│       │   └── TokenService.cs
│       ├── Providers/
│       │   ├── IAccessTokenProvider.cs
│       │   ├── AccessTokenProvider.cs
│       │   ├── IBeatportAccessTokenProvider.cs
│       │   └── BeatportAccessTokenProvider.cs
│       ├── Downloaders/
│       │   ├── IChromiumDownloader.cs
│       │   └── ChromiumDownloader.cs
│       ├── Options/
│       │   ├── BeatportCredentials.cs
│       │   └── ChromiumDownloaderOptions.cs
│       ├── Protos/
│       │   └── token.proto
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── Beatport2Rss.TokenProvider.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       └── appsettings.Development.json
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
├── .env.example
├── README.md
└── .gitignore
```

## Performance Considerations

- **Startup Time**: ~30-60s (depending on network) due to Chromium download on first start
  - Subsequent starts: ~2-3s (if Chromium cached)
- **Memory**: Token cached in-memory (~5KB per token), plus Chromium process (~150-200MB)
- **CPU**: Moderate during Chromium download, minimal during token serving
- **Network**: Required for Chromium download at startup and Beatport API calls
- **Request Latency**: <100ms for cached tokens, ~5-10s for token refresh (requires browser automation)

## License

Part of the Beatport2Rss project
