FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SoPorHoje.Api/SoPorHoje.Api.csproj", "SoPorHoje.Api/"]
COPY ["SoPorHoje.Scraper/SoPorHoje.Scraper.csproj", "SoPorHoje.Scraper/"]
RUN dotnet restore "SoPorHoje.Api/SoPorHoje.Api.csproj"
COPY . .
RUN dotnet publish "SoPorHoje.Api/SoPorHoje.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY ["data/daily_reflections_pt-br.json", "data/"]
ENTRYPOINT ["dotnet", "SoPorHoje.Api.dll"]
