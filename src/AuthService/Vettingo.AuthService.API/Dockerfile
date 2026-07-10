FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/AuthService/Vettingo.AuthService.API/Vettingo.AuthService.API.csproj", "src/AuthService/Vettingo.AuthService.API/"]
COPY ["src/AuthService/Vettingo.AuthService.Application/Vettingo.AuthService.Application.csproj", "src/AuthService/Vettingo.AuthService.Application/"]
COPY ["src/AuthService/Vettingo.AuthService.Domain/Vettingo.AuthService.Domain.csproj", "src/AuthService/Vettingo.AuthService.Domain/"]
COPY ["src/AuthService/Vettingo.AuthService.Infrastructure/Vettingo.AuthService.Infrastructure.csproj", "src/AuthService/Vettingo.AuthService.Infrastructure/"]
COPY ["src/AuthService/Vettingo.AuthService.Persistence/Vettingo.AuthService.Persistence.csproj", "src/AuthService/Vettingo.AuthService.Persistence/"]
RUN dotnet restore "src/AuthService/Vettingo.AuthService.API/Vettingo.AuthService.API.csproj"

COPY ["src/AuthService/", "src/AuthService/"]
WORKDIR "/src/src/AuthService/Vettingo.AuthService.API"
RUN dotnet publish "Vettingo.AuthService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.AuthService.API.dll"]
