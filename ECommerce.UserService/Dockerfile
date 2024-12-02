#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Set the environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ECommerce.UserService/ECommerce.UserService.csproj", "ECommerce.UserService/"]
COPY ["ECommerce.UserService.BusinessLogic/ECommerce.UserService.BusinessLogic.csproj", "ECommerce.UserService.BusinessLogic/"]
COPY ["ECommerce.UserService.DataAccess/ECommerce.UserService.DataAccess.csproj", "ECommerce.UserService.DataAccess/"]
COPY ["ECommerce.UserService.Model/ECommerce.UserService.Models.csproj", "ECommerce.UserService.Model/"]
COPY ["ECommerce.UserService.Utility/ECommerce.UserService.Utility.csproj", "ECommerce.UserService.Utility/"]
RUN dotnet restore "./ECommerce.UserService/ECommerce.UserService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./ECommerce.UserService/ECommerce.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ECommerce.UserService/ECommerce.UserService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ECommerce.UserService.dll"]
