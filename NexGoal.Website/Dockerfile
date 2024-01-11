FROM hub.nexdev.net/base/dotnet/core/sdk:7.0-alpine AS builder

WORKDIR /src

COPY ["NexGoal.Resources/NexGoal.Resources.csproj", "NexGoal.Resources/"]
COPY ["NexGoal.Core/NexGoal.Core.csproj", "NexGoal.Core/"]
COPY ["NexGoal.Web.Core/NexGoal.Web.Core.csproj", "NexGoal.Web.Core/"]
COPY ["NexGoal.Models/NexGoal.Models.csproj", "NexGoal.Models/"]
COPY ["NexGoal.ViewModels/NexGoal.ViewModels.csproj", "NexGoal.ViewModels/"]
COPY ["NexGoal.Criteria/NexGoal.Criteria.csproj", "NexGoal.Criteria/"]
COPY ["NexGoal.Repositories/NexGoal.Repositories.csproj", "NexGoal.Repositories/"]
COPY ["NexGoal.Services/NexGoal.Services.csproj", "NexGoal.Services/"]
COPY ["NexGoal.AutoService/NexGoal.AutoService.csproj", "NexGoal.AutoService/"]
COPY ["NexGoal.Controllers/NexGoal.Controllers.csproj", "NexGoal.Controllers/"]
COPY ["NexGoal.Website/NexGoal.Website.csproj", "NexGoal.Website/"]

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n "NexGoal.Website/NexGoal.Website.csproj"

COPY ["NexGoal.Resources/", "NexGoal.Resources/"]
COPY ["NexGoal.Core/", "NexGoal.Core/"]
COPY ["NexGoal.Web.Core/", "NexGoal.Web.Core/"]
COPY ["NexGoal.Models/", "NexGoal.Models/"]
COPY ["NexGoal.ViewModels/", "NexGoal.ViewModels/"]
COPY ["NexGoal.Criteria/", "NexGoal.Criteria/"]
COPY ["NexGoal.Repositories/", "NexGoal.Repositories/"]
COPY ["NexGoal.Services/", "NexGoal.Services/"]
COPY ["NexGoal.AutoService/", "NexGoal.AutoService/"]
COPY ["NexGoal.Controllers/", "NexGoal.Controllers/"]
COPY ["NexGoal.Website/", "NexGoal.Website/"]
COPY ["SonarQube/", "SonarQube/"]
WORKDIR /src/NexGoal.Website

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-slim AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

RUN apt-get update \
  && apt-get install -y --allow-unauthenticated libgdiplus \
  && rm -rf /var/lib/apt/lists/* /var/cache/apt/archives/*

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .
ENV TZ="Asia/Bangkok"

ENTRYPOINT ["dotnet", "NexGoal.Website.dll"]