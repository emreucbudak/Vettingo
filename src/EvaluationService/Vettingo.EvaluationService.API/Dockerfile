FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/EvaluationService/Vettingo.EvaluationService.API/Vettingo.EvaluationService.API.csproj", "src/EvaluationService/Vettingo.EvaluationService.API/"]
COPY ["src/EvaluationService/Vettingo.EvaluationService.Application/Vettingo.EvaluationService.Application.csproj", "src/EvaluationService/Vettingo.EvaluationService.Application/"]
COPY ["src/EvaluationService/Vettingo.EvaluationService.Domain/Vettingo.EvaluationService.Domain.csproj", "src/EvaluationService/Vettingo.EvaluationService.Domain/"]
COPY ["src/EvaluationService/Vettingo.EvaluationService.Infrastructure/Vettingo.EvaluationService.Infrastructure.csproj", "src/EvaluationService/Vettingo.EvaluationService.Infrastructure/"]
COPY ["src/EvaluationService/Vettingo.EvaluationService.Persistence/Vettingo.EvaluationService.Persistence.csproj", "src/EvaluationService/Vettingo.EvaluationService.Persistence/"]
RUN dotnet restore "src/EvaluationService/Vettingo.EvaluationService.API/Vettingo.EvaluationService.API.csproj"

COPY ["src/EvaluationService/", "src/EvaluationService/"]
WORKDIR "/src/src/EvaluationService/Vettingo.EvaluationService.API"
RUN dotnet publish "Vettingo.EvaluationService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.EvaluationService.API.dll"]
