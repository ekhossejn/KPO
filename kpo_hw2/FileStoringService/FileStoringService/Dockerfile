FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY Common/Common/Common.csproj Common/Common/
COPY FileStoringService/FileStoringService/FileStoringService.csproj FileStoringService/

RUN dotnet restore FileStoringService/FileStoringService.csproj

COPY Common/Common/ Common/Common/
COPY FileStoringService/FileStoringService/ FileStoringService/

WORKDIR /src/FileStoringService
RUN dotnet build FileStoringService.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/FileStoringService
RUN dotnet publish FileStoringService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/FileStorage
ENTRYPOINT ["dotnet", "FileStoringService.dll"]
