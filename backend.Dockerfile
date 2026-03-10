# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy files from projects folders
COPY ["src/BankMap.WebApi/BankMap.WebApi.csproj", "src/BankMap.WebApi/"]
COPY ["src/BankMap.Application/BankMap.Application.csproj", "src/BankMap.Application/"]
COPY ["src/BankMap.Infrastructure/BankMap.Infrastructure.csproj", "src/BankMap.Infrastructure/"]
COPY ["src/BankMap.Domain/BankMap.Domain.csproj", "src/BankMap.Domain/"]

# Restore dependencies
RUN dotnet restore "src/BankMap.WebApi/BankMap.WebApi.csproj"

# Copy all contains of layers
COPY src/ ./src/

# Build main project (WebApi)
WORKDIR "/src/src/BankMap.WebApi"
RUN dotnet publish "BankMap.WebApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BankMap.WebApi.dll"]