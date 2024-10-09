#ARG CI_REGISTRY_IMAGE

#FROM $CI_REGISTRY_IMAGE:builder AS deps

#RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

#ENV NUGET_PACKAGES=/src/packages

#COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

WORKDIR /src

COPY ["NexGoal.Resources/NexGoal.Resources.csproj", "NexGoal.Resources/"]
COPY ["NexGoal.Core/NexGoal.Core.csproj", "NexGoal.Core/"]
COPY ["NexGoal.BackgroundJob/NexGoal.BackgroundJob.csproj", "NexGoal.BackgroundJob/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n -r linux-x64 "NexGoal.BackgroundJob/NexGoal.BackgroundJob.csproj"

COPY ["NexGoal.Resources/", "NexGoal.Resources/"]
COPY ["NexGoal.Core/", "NexGoal.Core/"]
COPY ["NexGoal.BackgroundJob/", "NexGoal.BackgroundJob/"]

WORKDIR "/src/NexGoal.BackgroundJob"

RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-slim AS base

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="Asia/Bangkok"

ENTRYPOINT ["dotnet", "NexGoal.BackgroundJob.dll"]