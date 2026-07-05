# Interceptor Service

A gRPC microservice that provides Beatport access tokens by intercepting authentication responses from the Beatport website using headless browser automation.

## Overview

The Interceptor service is a specialized token provider that intercepts Beatport access tokens from HTTP responses during authentication. It exposes a gRPC endpoint (`GetToken`) that returns cached tokens with automatic refresh capability.

### Key Features

- **Token Interception**: Uses PuppeteerSharp with Chromium to automate Beatport login and intercept access tokens from HTTP responses
- **Intelligent Caching**: Maintains in-memory token cache with automatic expiration based on token TTL
- **Resilience**: Includes retry logic (up to 3 attempts) and 5-minute operation timeouts via Polly resilience pipelines
- **Thread-Safe Operations**: Semaphore-based locking ensures safe concurrent access
- **Health Checks**: Exposes health check endpoint for Kubernetes/container orchestration monitoring

## Architecture

### Core Components

- **TokenGrpcService**: gRPC service exposing `GetToken()` RPC that returns access tokens
- **BeatportAccessTokenInterceptor**: Core interceptor that uses Chromium browser automation to:
  - Navigate to Beatport login page
  - Submit credentials
  - Intercept HTTP responses to extract access tokens
  - Extract token expiration time
- **AccessTokenProvider**: Wrapper that manages token caching, expiration, and refresh logic
- **ChromiumDownloader**: Handles automated download and installation of Chromium browser executable

### Technology Stack

- **.NET 10** with ASP.NET Core
- **gRPC** (with reflection support) for service communication
- **PuppeteerSharp** for headless browser automation
- **Polly** for resilience patterns (retries, timeouts)
- **Docker** for containerization with multi-stage builds

### gRPC Interface

The service defines a simple gRPC contract shared from `common/protos/TokenService.proto`:

```protobuf
service GrpcBeatportAccessTokenService {
  rpc GetToken (GetTokenRequest) returns (GetTokenResponse);
}

message GetTokenRequest {
}

message GetTokenResponse {
  string access_token = 1;
}
```

## Local Development

### Prerequisites

- Docker and Docker Compose (recommended)
- .NET 10 SDK (for local compilation only)
- Beatport credentials with valid account
- 4+ GB RAM (for Chromium runtime)

### Setup

1. From the solution root, configure environment variables:
   ```bash
   cp docker/.env.example docker/.env
   ```

2. Update `docker/.env` with your Beatport credentials:
   ```bash
   BEATPORT_USERNAME=your_beatport_email@example.com
   BEATPORT_PASSWORD=your_beatport_password
   ```

3. Start the service via Docker Compose:
   ```bash
   docker-compose -f docker/docker-compose.yml up token-interceptor
   ```

### Running Locally (without Docker)

If developing locally on Windows:

```bash
cd services/Interceptor/src/Beatport2Rss.Interceptor
dotnet run
```

The service will:
- Listen on HTTP/2 (gRPC default): `https://localhost:5001`
- Expose gRPC reflection at `https://localhost:5001/grpc.reflection.v1.Reflection/ServerReflection`
- Expose health checks at `http://localhost:8080/health`

## Testing the Service

### Using grpcurl

If you have `grpcurl` installed, test the service:

```bash
grpcurl -plaintext -d '{}' localhost:5000 Beatport2Rss.Interceptor.TokenService/GetToken
```

### Observing Behavior

The service logs browser automation steps and token interception events. To see verbose logs:

```bash
# Via docker-compose environment
docker-compose logs -f token-interceptor
```

## Configuration

Key settings are managed through `appsettings.json` and environment variables:

```json
{
  "BeatportCredentials": {
    "Username": "${BEATPORT_USERNAME}",
    "Password": "${BEATPORT_PASSWORD}"
  },
  "ChromiumDownloaderOptions": {
    "BaseAddress": "https://download.example.com"
  }
}
```

## Troubleshooting

### Chromium Download Fails

- Ensure the container has internet access
- Check that `/app/chromium` directory is writable
- Verify `ChromiumDownloaderOptions.BaseAddress` is reachable

### Token Interception Fails

- Verify Beatport credentials are valid
- Check if Beatport has changed their login flow (may require updating interceptor logic)
- Review logs for JavaScript errors during page navigation

### gRPC Connection Issues

- Ensure the container port (default 5000) is properly exposed
- Verify gRPC client is using HTTP/2 protocol
- Check TLS certificate configuration for production deployments

## Production Considerations

- The service maintains a single Chromium process for token interception
- Token cache is in-memory; service restart clears cache
- For high-availability deployments, consider:
  - Running multiple replicas with a shared token cache (Redis)
  - Adding a token cache warming service
  - Implementing circuit breakers for Beatport API changes
- Monitor Chromium process memory usage; set container limits accordingly

## Docker Deployment

### Build Image

```bash
docker build -f services/Interceptor/src/Beatport2Rss.Interceptor/Dockerfile -t beatport-token-interceptor .
```

### Run Container

```bash
docker run -p 5000:5000 \
  -e BeatportCredentials__Username=your_username \
  -e BeatportCredentials__Password=your_password \
  beatport-token-interceptor
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

## Development

### Project Structure

```
services/Interceptor/
├── src/
│   └── Beatport2Rss.Interceptor/
│       ├── GrpcServices/
│       │   └── TokenService.cs
│       ├── Services/
│       │   ├── AccessTokenProvider.cs
│       │   ├── BeatportAccessTokenInterceptor.cs
│       │   ├── ChromiumDownloader.cs
│       │   └── Interfaces/
│       │       ├── IAccessTokenProvider.cs
│       │       ├── IBeatportAccessTokenInterceptor.cs
│       │       └── IChromiumDownloader.cs
│       ├── HostedServices/
│       │   └── ChromiumWarmup.cs
│       ├── Options/
│       │   ├── BeatportCredentials.cs
│       │   └── ChromiumDownloaderOptions.cs
│       ├── Protos/
│       │   └── token.proto
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── chromium/
│       │   └── (downloaded Chromium executable)
│       ├── Beatport2Rss.TokenInterceptor.csproj
│       ├── Dockerfile
│       ├── Program.cs
│       ├── appsettings.json
│       ├── bin/
│       │   └── (build output)
│       └── obj/
│           └── (build output)
└── README.md
```

### Key Directories

- **GrpcServices/**: Contains the gRPC service implementation (`TokenGrpcService`)
- **Services/**: Core business logic including token interception, caching, and browser automation
- **HostedServices/**: Background services like Chromium warmup that run on startup
- **Options/**: Configuration option classes bound from `appsettings.json` and environment variables
- **Protos/**: Protocol buffer definitions for gRPC contracts
- **chromium/**: Runtime directory where Chromium executable is downloaded and cached

## Performance Considerations

- **Startup Time**: ~30-60s (depending on network) due to Chromium download on first start
  - Subsequent starts: ~2-3s (if Chromium cached)
- **Memory**: Token cached in-memory (~5KB per token), plus Chromium process (~150-200MB)
- **CPU**: Moderate during Chromium download, minimal during token serving
- **Network**: Required for Chromium download at startup and Beatport API calls
- **Request Latency**: <100ms for cached tokens, ~5-10s for token refresh (requires browser automation)

## License

Part of the Beatport2Rss project
