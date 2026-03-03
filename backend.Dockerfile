# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy files from projects folders
COPY ["BankMap.WebApi/BankMap.WebApi.csproj", "BankMap.WebApi/"]
COPY ["BankMap.Application/BankMap.Application.csproj", "BankMap.Application/"]
COPY ["BankMap.Infrastructure/BankMap.Infrastructure.csproj", "BankMap.Infrastructure/"]
COPY ["BankMap.Domain/BankMap.Domain.csproj", "BankMap.Domain/"]

# Restore dependencies
RUN dotnet restore "BankMap.WebApi/BankMap.WebApi.csproj"

# Copy all contains of layers
COPY . .

# Build main project (WebApi)
WORKDIR "/src/BankMap.WebApi"
RUN dotnet publish "BankMap.WebApi.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "BankMap.WebApi.dll"]