# Base image for .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["src/APIGateway.API/APIGateway.API.csproj", "APIGateway.API/"]
WORKDIR /src/APIGateway.API
RUN dotnet restore "APIGateway.API.csproj"

# Copy entire source and build
COPY . .
WORKDIR /src/APIGateway.API
RUN dotnet build "APIGateway.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "APIGateway.API.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /src
COPY --from=publish /app/publish .
COPY ["src/APIGateway.API/appsettings.json", "src/APIGateway.API/appsettings.Development.json", "src/APIGateway.API/ocelot.json", "./"]

ENTRYPOINT ["dotnet", "APIGateway.API.dll"]
