FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "Directory.Packages.props", "nuget.config", "./"]
COPY ["web/src/Beatport2Rss.Web/Beatport2Rss.Web.csproj", "web/src/Beatport2Rss.Web/"]
RUN dotnet restore "web/src/Beatport2Rss.Web/Beatport2Rss.Web.csproj"

COPY ["web/src/Beatport2Rss.Web/", "web/src/Beatport2Rss.Web/"]
RUN dotnet publish "web/src/Beatport2Rss.Web/Beatport2Rss.Web.csproj" --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Beatport2Rss.Web.dll"]
