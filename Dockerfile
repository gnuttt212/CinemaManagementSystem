# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CinemaManagementSystem.sln", "./"]
COPY ["Cinema.Web/Cinema.Web.csproj", "Cinema.Web/"]
COPY ["Cinema.BUS/Cinema.BUS.csproj", "Cinema.BUS/"]
COPY ["Cinema.DAL/Cinema.DAL.csproj", "Cinema.DAL/"]
COPY ["Cinema.DTO/Cinema.DTO.csproj", "Cinema.DTO/"]

RUN dotnet restore "Cinema.Web/Cinema.Web.csproj"

COPY . .
RUN dotnet publish "Cinema.Web/Cinema.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for Docker HEALTHCHECK
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
RUN mkdir -p /app/wwwroot/images/phim /app/logs

# Docker HEALTHCHECK — liveness probe
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
  CMD curl --fail http://localhost:8080/healthz || exit 1

ENTRYPOINT ["dotnet", "Cinema.Web.dll"]
