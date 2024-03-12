#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-slim AS base
USER app
WORKDIR /app

FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["NexGoal.BackgroundJob/NexGoal.BackgroundJob.csproj", "NexGoal.BackgroundJob/"]
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "./NexGoal.BackgroundJob/./NexGoal.BackgroundJob.csproj"
COPY . .
WORKDIR "/src/NexGoal.BackgroundJob"
RUN dotnet build "./NexGoal.BackgroundJob.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NexGoal.BackgroundJob.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NexGoal.BackgroundJob.dll"]