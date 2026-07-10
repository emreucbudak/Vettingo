FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/NotificationService/Vettingo.NotificationService.API/Vettingo.NotificationService.API.csproj", "src/NotificationService/Vettingo.NotificationService.API/"]
COPY ["src/NotificationService/Vettingo.NotificationService.Application/Vettingo.NotificationService.Application.csproj", "src/NotificationService/Vettingo.NotificationService.Application/"]
COPY ["src/NotificationService/Vettingo.NotificationService.Domain/Vettingo.NotificationService.Domain.csproj", "src/NotificationService/Vettingo.NotificationService.Domain/"]
COPY ["src/NotificationService/Vettingo.NotificationService.Infrastructure/Vettingo.NotificationService.Infrastructure.csproj", "src/NotificationService/Vettingo.NotificationService.Infrastructure/"]
COPY ["src/NotificationService/Vettingo.NotificationService.Persistence/Vettingo.NotificationService.Persistence.csproj", "src/NotificationService/Vettingo.NotificationService.Persistence/"]
RUN dotnet restore "src/NotificationService/Vettingo.NotificationService.API/Vettingo.NotificationService.API.csproj"

COPY ["src/NotificationService/", "src/NotificationService/"]
WORKDIR "/src/src/NotificationService/Vettingo.NotificationService.API"
RUN dotnet publish "Vettingo.NotificationService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.NotificationService.API.dll"]
