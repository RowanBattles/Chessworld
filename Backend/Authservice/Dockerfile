# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8084

# Install krb5-libs for MailKit (GSSAPI support)
RUN apt-get update && apt-get install -y libkrb5-3 && rm -rf /var/lib/apt/lists/*

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/Authservice.API/Authservice.API.csproj", "src/Authservice.API/"]
RUN dotnet restore "src/Authservice.API/Authservice.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Authservice.API"
RUN dotnet build "Authservice.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Authservice.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8084
ENTRYPOINT ["dotnet", "Authservice.API.dll"]