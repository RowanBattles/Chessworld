# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base stage: ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

# Build stage: using the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies.
COPY ["src/Matchmaking.API/Matchmaking.API.csproj", "src/Matchmaking.API/"]
RUN dotnet restore "src/Matchmaking.API/Matchmaking.API.csproj"

# Copy the rest of the code
COPY . .

# Change working directory to the project folder
WORKDIR "/src/src/Matchmaking.API"
RUN dotnet build "Matchmaking.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage: produce the final published output
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Matchmaking.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: runtime container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Matchmaking.API.dll"]