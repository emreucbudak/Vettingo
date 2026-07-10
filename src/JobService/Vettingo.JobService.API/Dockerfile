FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/JobService/Vettingo.JobService.API/Vettingo.JobService.API.csproj", "src/JobService/Vettingo.JobService.API/"]
COPY ["src/JobService/Vettingo.JobService.Application/Vettingo.JobService.Application.csproj", "src/JobService/Vettingo.JobService.Application/"]
COPY ["src/JobService/Vettingo.JobService.Domain/Vettingo.JobService.Domain.csproj", "src/JobService/Vettingo.JobService.Domain/"]
COPY ["src/JobService/Vettingo.JobService.Infrastructure/Vettingo.JobService.Infrastructure.csproj", "src/JobService/Vettingo.JobService.Infrastructure/"]
COPY ["src/JobService/Vettingo.JobService.Persistence/Vettingo.JobService.Persistence.csproj", "src/JobService/Vettingo.JobService.Persistence/"]
RUN dotnet restore "src/JobService/Vettingo.JobService.API/Vettingo.JobService.API.csproj"

COPY ["src/JobService/", "src/JobService/"]
WORKDIR "/src/src/JobService/Vettingo.JobService.API"
RUN dotnet publish "Vettingo.JobService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.JobService.API.dll"]
