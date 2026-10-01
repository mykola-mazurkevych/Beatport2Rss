FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Beatport2Rss.slnx", "."]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
COPY ["nuget.config", "."]

COPY ["services/builder/src/Beatport2Rss.Builder.Worker/Beatport2Rss.Builder.Worker.csproj", "services/builder/src/Beatport2Rss.Builder.Worker/"]
COPY ["services/builder/src/Beatport2Rss.Builder.Application/Beatport2Rss.Builder.Application.csproj", "services/builder/src/Beatport2Rss.Builder.Application/"]
COPY ["services/builder/src/Beatport2Rss.Builder.Domain/Beatport2Rss.Builder.Domain.csproj", "services/builder/src/Beatport2Rss.Builder.Domain/"]
COPY ["services/builder/src/Beatport2Rss.Builder.Infrastructure/Beatport2Rss.Builder.Infrastructure.csproj", "services/builder/src/Beatport2Rss.Builder.Infrastructure/"]
COPY ["services/builder/src/Beatport2Rss.Builder.Jobs/Beatport2Rss.Builder.Jobs.csproj", "services/builder/src/Beatport2Rss.Builder.Jobs/"]
COPY ["common/src/Beatport2Rss.Common.EntityFrameworkCore/Beatport2Rss.Common.EntityFrameworkCore.csproj", "common/src/Beatport2Rss.Common.EntityFrameworkCore/"]
COPY ["common/src/Beatport2Rss.Common.IntegrationEvents/Beatport2Rss.Common.IntegrationEvents.csproj", "common/src/Beatport2Rss.Common.IntegrationEvents/"]
COPY ["common/src/Beatport2Rss.Common.Messaging/Beatport2Rss.Common.Messaging.csproj", "common/src/Beatport2Rss.Common.Messaging/"]
COPY ["common/src/Beatport2Rss.Common.Miscellaneous/Beatport2Rss.Common.Miscellaneous.csproj", "common/src/Beatport2Rss.Common.Miscellaneous/"]
COPY ["common/src/Beatport2Rss.Common.RabbitMQ/Beatport2Rss.Common.RabbitMQ.csproj", "common/src/Beatport2Rss.Common.RabbitMQ/"]
COPY ["common/src/Beatport2Rss.Common.SharedKernel/Beatport2Rss.Common.SharedKernel.csproj", "common/src/Beatport2Rss.Common.SharedKernel/"]
COPY ["common/src/Beatport2Rss.Common.Validation/Beatport2Rss.Common.Validation.csproj", "common/src/Beatport2Rss.Common.Validation/"]
COPY ["tools/src/Beatport2Rss.SourceGenerator/Beatport2Rss.SourceGenerator.csproj", "tools/src/Beatport2Rss.SourceGenerator/"]

RUN dotnet restore "services/builder/src/Beatport2Rss.Builder.Worker/Beatport2Rss.Builder.Worker.csproj"

COPY ["services/builder/src/", "services/builder/src/"]
COPY ["common/src/Beatport2Rss.Common.EntityFrameworkCore/", "common/src/Beatport2Rss.Common.EntityFrameworkCore/"]
COPY ["common/src/Beatport2Rss.Common.IntegrationEvents/", "common/src/Beatport2Rss.Common.IntegrationEvents/"]
COPY ["common/src/Beatport2Rss.Common.Messaging/", "common/src/Beatport2Rss.Common.Messaging/"]
COPY ["common/src/Beatport2Rss.Common.Miscellaneous/", "common/src/Beatport2Rss.Common.Miscellaneous/"]
COPY ["common/src/Beatport2Rss.Common.RabbitMQ/", "common/src/Beatport2Rss.Common.RabbitMQ/"]
COPY ["common/src/Beatport2Rss.Common.SharedKernel/", "common/src/Beatport2Rss.Common.SharedKernel/"]
COPY ["common/src/Beatport2Rss.Common.Validation/", "common/src/Beatport2Rss.Common.Validation/"]
COPY ["tools/src/Beatport2Rss.SourceGenerator/", "tools/src/Beatport2Rss.SourceGenerator/"]

RUN dotnet publish "services/builder/src/Beatport2Rss.Builder.Worker/Beatport2Rss.Builder.Worker.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Beatport2Rss.Builder.Worker.dll"]