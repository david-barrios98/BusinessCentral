# syntax=docker/dockerfile:1
ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS restore
WORKDIR /src
COPY BusinessCentral.Api.sln ./
COPY BusinessCentral.Domain/BusinessCentral.Domain.csproj BusinessCentral.Domain/
COPY BusinessCentral.Application/BusinessCentral.Application.csproj BusinessCentral.Application/
COPY BusinessCentral.Infrastructure/BusinessCentral.Infrastructure.csproj BusinessCentral.Infrastructure/
COPY BusinessCentral.Api/BusinessCentral.Api.csproj BusinessCentral.Api/
RUN dotnet restore BusinessCentral.Api/BusinessCentral.Api.csproj

FROM restore AS publish
COPY BusinessCentral.Domain/ BusinessCentral.Domain/
COPY BusinessCentral.Application/ BusinessCentral.Application/
COPY BusinessCentral.Infrastructure/ BusinessCentral.Infrastructure/
COPY BusinessCentral.Api/ BusinessCentral.Api/
RUN dotnet publish BusinessCentral.Api/BusinessCentral.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BusinessCentral.Api.dll"]
