FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Beatport2Rss.slnx", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]

COPY ["services/collector/src/Beatport2Rss.Collector.Application/Beatport2Rss.Collector.Application.csproj", "services/collector/src/Beatport2Rss.Collector.Application/"]
COPY ["services/collector/src/Beatport2Rss.Collector.Domain/Beatport2Rss.Collector.Domain.csproj", "services/collector/src/Beatport2Rss.Collector.Domain/"]
COPY ["services/collector/src/Beatport2Rss.Collector.Infrastructure/Beatport2Rss.Collector.Infrastructure.csproj", "services/collector/src/Beatport2Rss.Collector.Infrastructure/"]
COPY ["services/collector/src/Beatport2Rss.Collector.Jobs/Beatport2Rss.Collector.Jobs.csproj", "services/collector/src/Beatport2Rss.Collector.Jobs/"]
COPY ["services/collector/src/Beatport2Rss.Collector.Worker/Beatport2Rss.Collector.Worker.csproj", "services/collector/src/Beatport2Rss.Collector.Worker/"]
COPY ["common/src/Beatport2Rss.Common.Beatport/Beatport2Rss.Common.Beatport.csproj", "common/src/Beatport2Rss.Common.Beatport/"]
COPY ["common/src/Beatport2Rss.Common.BeatportTokenProvider/Beatport2Rss.Common.BeatportTokenProvider.csproj", "common/src/Beatport2Rss.Common.BeatportTokenProvider/"]
COPY ["common/src/Beatport2Rss.Common.Miscellaneous/Beatport2Rss.Common.Miscellaneous.csproj", "common/src/Beatport2Rss.Common.Miscellaneous/"]
COPY ["common/protos/", "common/protos/"]
COPY ["common/src/Beatport2Rss.Common.Domain/Beatport2Rss.Common.Domain.csproj", "common/src/Beatport2Rss.Common.Domain/"]
COPY ["common/src/Beatport2Rss.Common.EntityFrameworkCore/Beatport2Rss.Common.EntityFrameworkCore.csproj", "common/src/Beatport2Rss.Common.EntityFrameworkCore/"]
COPY ["common/src/Beatport2Rss.Common.FluentResults/Beatport2Rss.Common.FluentResults.csproj", "common/src/Beatport2Rss.Common.FluentResults/"]
COPY ["common/src/Beatport2Rss.Common.IntegrationEvents/Beatport2Rss.Common.IntegrationEvents.csproj", "common/src/Beatport2Rss.Common.IntegrationEvents/"]
COPY ["common/src/Beatport2Rss.Common.SharedKernel/Beatport2Rss.Common.SharedKernel.csproj", "common/src/Beatport2Rss.Common.SharedKernel/"]
COPY ["common/src/Beatport2Rss.Common.Wolverine/Beatport2Rss.Common.Wolverine.csproj", "common/src/Beatport2Rss.Common.Wolverine/"]

RUN dotnet restore "services/collector/src/Beatport2Rss.Collector.Worker/Beatport2Rss.Collector.Worker.csproj"

COPY ["services/collector/src/", "services/collector/src/"]
COPY ["common/src/Beatport2Rss.Common.Beatport/", "common/src/Beatport2Rss.Common.Beatport/"]
COPY ["common/src/Beatport2Rss.Common.BeatportTokenProvider/", "common/src/Beatport2Rss.Common.BeatportTokenProvider/"]
COPY ["common/src/Beatport2Rss.Common.Miscellaneous/", "common/src/Beatport2Rss.Common.Miscellaneous/"]
COPY ["common/src/Beatport2Rss.Common.Domain/", "common/src/Beatport2Rss.Common.Domain/"]
COPY ["common/src/Beatport2Rss.Common.EntityFrameworkCore/", "common/src/Beatport2Rss.Common.EntityFrameworkCore/"]
COPY ["common/src/Beatport2Rss.Common.FluentResults/", "common/src/Beatport2Rss.Common.FluentResults/"]
COPY ["common/src/Beatport2Rss.Common.IntegrationEvents/", "common/src/Beatport2Rss.Common.IntegrationEvents/"]
COPY ["common/src/Beatport2Rss.Common.SharedKernel/", "common/src/Beatport2Rss.Common.SharedKernel/"]
COPY ["common/src/Beatport2Rss.Common.Wolverine/", "common/src/Beatport2Rss.Common.Wolverine/"]

RUN dotnet publish "services/collector/src/Beatport2Rss.Collector.Worker/Beatport2Rss.Collector.Worker.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Beatport2Rss.Collector.Worker.dll"]