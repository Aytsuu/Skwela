# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy all .csproj files to restore dependencies
COPY ["Skwela.API/Skwela.API.csproj", "Skwela.API/"]
COPY ["Skwela.Application/Skwela.Application.csproj", "Skwela.Application/"]
COPY ["Skwela.Domain/Skwela.Domain.csproj", "Skwela.Domain/"]
COPY ["Skwela.Infrastructure/Skwela.Infrastructure.csproj", "Skwela.Infrastructure/"]
COPY ["Skwela.sln", "./"]

RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish "Skwela.API/Skwela.API.csproj" -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Cloudflare Tunnel will point to this port
EXPOSE 8080
ENTRYPOINT ["dotnet", "Skwela.API.dll"]