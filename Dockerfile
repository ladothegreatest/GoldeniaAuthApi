# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY GoldeniaAuthApi/*.csproj ./GoldeniaAuthApi/
RUN dotnet restore

# Copy everything else
COPY . .

# Publish the project
RUN dotnet publish GoldeniaAuthApi/ -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 10000

ENTRYPOINT ["dotnet", "GoldeniaAuthApi.dll"]
