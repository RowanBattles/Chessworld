# Base stage: ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage: using the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies.
# Note: The path is relative to the build context (GameService folder).
COPY ["src/GameService.API/GameService.API.csproj", "src/GameService.API/"]
RUN dotnet restore "src/GameService.API/GameService.API.csproj"

# Copy the rest of the code
COPY . .

# Change working directory to the project folder (notice the repeated 'src' because the build context already has a 'src' folder)
WORKDIR "/src/src/GameService.API"
RUN dotnet build "GameService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage: produce the final published output
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GameService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: runtime container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameService.API.dll"]