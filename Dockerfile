# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["ReadyStatusApp.csproj", "./"]
RUN dotnet restore

# Copy source code and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port (Cloud Run uses PORT environment variable)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "ReadyStatusApp.dll"]
