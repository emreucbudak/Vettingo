FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/AnalyticsService/Vettingo.AnalyticsService.API/Vettingo.AnalyticsService.API.csproj", "src/AnalyticsService/Vettingo.AnalyticsService.API/"]
COPY ["src/AnalyticsService/Vettingo.AnalyticsService.Application/Vettingo.AnalyticsService.Application.csproj", "src/AnalyticsService/Vettingo.AnalyticsService.Application/"]
COPY ["src/AnalyticsService/Vettingo.AnalyticsService.Domain/Vettingo.AnalyticsService.Domain.csproj", "src/AnalyticsService/Vettingo.AnalyticsService.Domain/"]
COPY ["src/AnalyticsService/Vettingo.AnalyticsService.Infrastructure/Vettingo.AnalyticsService.Infrastructure.csproj", "src/AnalyticsService/Vettingo.AnalyticsService.Infrastructure/"]
COPY ["src/AnalyticsService/Vettingo.AnalyticsService.Persistence/Vettingo.AnalyticsService.Persistence.csproj", "src/AnalyticsService/Vettingo.AnalyticsService.Persistence/"]
RUN dotnet restore "src/AnalyticsService/Vettingo.AnalyticsService.API/Vettingo.AnalyticsService.API.csproj"

COPY ["src/AnalyticsService/", "src/AnalyticsService/"]
WORKDIR "/src/src/AnalyticsService/Vettingo.AnalyticsService.API"
RUN dotnet publish "Vettingo.AnalyticsService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.AnalyticsService.API.dll"]
