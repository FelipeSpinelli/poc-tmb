FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY poc-tmb.sln ./
COPY Tmb.OrderManagementSystem.Api/Tmb.OrderManagementSystem.Api.csproj Tmb.OrderManagementSystem.Api/
COPY Tmb.OrderManagementSystem.Core/Tmb.OrderManagementSystem.Core.csproj Tmb.OrderManagementSystem.Core/

RUN dotnet restore "./Tmb.OrderManagementSystem.Api/./Tmb.OrderManagementSystem.Api.csproj"

COPY . .
WORKDIR "/src/Tmb.OrderManagementSystem.Api"
RUN dotnet build "./Tmb.OrderManagementSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tmb.OrderManagementSystem.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tmb.OrderManagementSystem.Api.dll"]