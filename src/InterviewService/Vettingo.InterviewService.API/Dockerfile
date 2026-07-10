FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/InterviewService/Vettingo.InterviewService.API/Vettingo.InterviewService.API.csproj", "src/InterviewService/Vettingo.InterviewService.API/"]
COPY ["src/InterviewService/Vettingo.InterviewService.Application/Vettingo.InterviewService.Application.csproj", "src/InterviewService/Vettingo.InterviewService.Application/"]
COPY ["src/InterviewService/Vettingo.InterviewService.Domain/Vettingo.InterviewService.Domain.csproj", "src/InterviewService/Vettingo.InterviewService.Domain/"]
COPY ["src/InterviewService/Vettingo.InterviewService.Infrastructure/Vettingo.InterviewService.Infrastructure.csproj", "src/InterviewService/Vettingo.InterviewService.Infrastructure/"]
COPY ["src/InterviewService/Vettingo.InterviewService.Persistence/Vettingo.InterviewService.Persistence.csproj", "src/InterviewService/Vettingo.InterviewService.Persistence/"]
RUN dotnet restore "src/InterviewService/Vettingo.InterviewService.API/Vettingo.InterviewService.API.csproj"

COPY ["src/InterviewService/", "src/InterviewService/"]
WORKDIR "/src/src/InterviewService/Vettingo.InterviewService.API"
RUN dotnet publish "Vettingo.InterviewService.API.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Vettingo.InterviewService.API.dll"]
