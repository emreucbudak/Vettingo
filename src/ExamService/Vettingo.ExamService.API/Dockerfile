FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/ExamService/Vettingo.ExamService.API/Vettingo.ExamService.API.csproj", "src/ExamService/Vettingo.ExamService.API/"]
COPY ["src/ExamService/Vettingo.ExamService.Application/Vettingo.ExamService.Application.csproj", "src/ExamService/Vettingo.ExamService.Application/"]
COPY ["src/ExamService/Vettingo.ExamService.Domain/Vettingo.ExamService.Domain.csproj", "src/ExamService/Vettingo.ExamService.Domain/"]
COPY ["src/ExamService/Vettingo.ExamService.Infrastructure/Vettingo.ExamService.Infrastructure.csproj", "src/ExamService/Vettingo.ExamService.Infrastructure/"]
COPY ["src/ExamService/Vettingo.ExamService.Persistence/Vettingo.ExamService.Persistence.csproj", "src/ExamService/Vettingo.ExamService.Persistence/"]
RUN dotnet restore "src/ExamService/Vettingo.ExamService.API/Vettingo.ExamService.API.csproj"

COPY ["src/ExamService/", "src/ExamService/"]
WORKDIR "/src/src/ExamService/Vettingo.ExamService.API"
RUN dotnet publish "Vettingo.ExamService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.ExamService.API.dll"]
