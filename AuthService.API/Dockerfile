# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution file first for better dependency resolution
COPY ["AuthService.sln", "./"]

# Copy all project files for better layer caching
# This allows Docker to cache the restore layer when only source code changes
COPY ["AuthService.API/AuthService.API.csproj", "AuthService.API/"]
COPY ["AuthService.Application/AuthService.Application.csproj", "AuthService.Application/"]
COPY ["AuthService.Infrastructure/AuthService.Infrastructure.csproj", "AuthService.Infrastructure/"]

# Restore all NuGet packages for all projects using the solution file
# This ensures all dependencies (direct and transitive) are restored
RUN dotnet restore "AuthService.sln" --verbosity quiet

# Copy the rest of the source code after restore for better caching
COPY . .

# Build the entire solution (all projects) to ensure dependencies are built correctly
# Build output goes to standard bin/obj directories for publish to find
WORKDIR "/src"
RUN dotnet build "AuthService.sln" -c $BUILD_CONFIGURATION --no-restore

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/AuthService.API"
# Publish the API project with all its dependencies
# Using --no-build to use the already built solution from previous step
RUN dotnet publish "AuthService.API.csproj" -c $BUILD_CONFIGURATION --no-build -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Create a non-root user for security (UID 1001 is compatible with Visual Studio)
# This UID works well with both Visual Studio debugging and production deployments
RUN groupadd -r -g 1001 appgroup && \
    useradd -r -u 1001 -g appgroup -s /bin/false appuser && \
    chown -R appuser:appgroup /app

# Switch to non-root user for better security
USER appuser

ENTRYPOINT ["dotnet", "AuthService.API.dll"]