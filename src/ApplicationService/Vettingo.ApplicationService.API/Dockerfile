FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/ApplicationService/Vettingo.ApplicationService.API/Vettingo.ApplicationService.API.csproj", "src/ApplicationService/Vettingo.ApplicationService.API/"]
COPY ["src/ApplicationService/Vettingo.ApplicationService.Application/Vettingo.ApplicationService.Application.csproj", "src/ApplicationService/Vettingo.ApplicationService.Application/"]
COPY ["src/ApplicationService/Vettingo.ApplicationService.Domain/Vettingo.ApplicationService.Domain.csproj", "src/ApplicationService/Vettingo.ApplicationService.Domain/"]
COPY ["src/ApplicationService/Vettingo.ApplicationService.Persistence/Vettingo.ApplicationService.Persistence.csproj", "src/ApplicationService/Vettingo.ApplicationService.Persistence/"]
RUN dotnet restore "src/ApplicationService/Vettingo.ApplicationService.API/Vettingo.ApplicationService.API.csproj"

COPY ["src/ApplicationService/", "src/ApplicationService/"]
WORKDIR "/src/src/ApplicationService/Vettingo.ApplicationService.API"
RUN dotnet publish "Vettingo.ApplicationService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.ApplicationService.API.dll"]
