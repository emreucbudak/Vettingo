FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/SubscriptionService/Vettingo.SubscriptionService.API/Vettingo.SubscriptionService.API.csproj", "src/SubscriptionService/Vettingo.SubscriptionService.API/"]
COPY ["src/SubscriptionService/Vettingo.SubscriptionService.Application/Vettingo.SubscriptionService.Application.csproj", "src/SubscriptionService/Vettingo.SubscriptionService.Application/"]
COPY ["src/SubscriptionService/Vettingo.SubscriptionService.Domain/Vettingo.SubscriptionService.Domain.csproj", "src/SubscriptionService/Vettingo.SubscriptionService.Domain/"]
COPY ["src/SubscriptionService/Vettingo.SubscriptionService.Infrastructure/Vettingo.SubscriptionService.Infrastructure.csproj", "src/SubscriptionService/Vettingo.SubscriptionService.Infrastructure/"]
COPY ["src/SubscriptionService/Vettingo.SubscriptionService.Persistence/Vettingo.SubscriptionService.Persistence.csproj", "src/SubscriptionService/Vettingo.SubscriptionService.Persistence/"]
RUN dotnet restore "src/SubscriptionService/Vettingo.SubscriptionService.API/Vettingo.SubscriptionService.API.csproj"

COPY ["src/SubscriptionService/", "src/SubscriptionService/"]
WORKDIR "/src/src/SubscriptionService/Vettingo.SubscriptionService.API"
RUN dotnet publish "Vettingo.SubscriptionService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.SubscriptionService.API.dll"]
