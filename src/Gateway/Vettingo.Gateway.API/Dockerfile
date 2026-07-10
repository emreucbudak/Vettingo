FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Gateway/Vettingo.Gateway.API/Vettingo.Gateway.API.csproj", "src/Gateway/Vettingo.Gateway.API/"]
RUN dotnet restore "src/Gateway/Vettingo.Gateway.API/Vettingo.Gateway.API.csproj"

COPY ["src/Gateway/", "src/Gateway/"]
WORKDIR "/src/src/Gateway/Vettingo.Gateway.API"
RUN dotnet publish "Vettingo.Gateway.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.Gateway.API.dll"]
