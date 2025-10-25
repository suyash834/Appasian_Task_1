# syntax=docker/dockerfile:1

# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj first (better layer caching)
COPY TaskManager.Api.csproj ./
RUN dotnet restore

# Copy the rest and publish
COPY . ./
RUN dotnet publish -c Release -o /app/out /p:UseAppHost=false

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Render provides $PORT; bind to it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

# Optional for local dev
EXPOSE 8080

# Bring in published output
COPY --from=build /app/out ./

# Start the API
ENTRYPOINT ["dotnet", "TaskManager.Api.dll"]
