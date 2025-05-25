FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY Common/Common/Common.csproj FileAnalysisService/Common/Common/
COPY FileAnalysisService/FileAnalysisService/FileAnalysisService.csproj FileAnalysisService/FileAnalysisService/

WORKDIR /src/FileAnalysisService/FileAnalysisService
RUN dotnet restore FileAnalysisService.csproj

WORKDIR /src
COPY Common/Common/ FileAnalysisService/Common/Common/
COPY FileAnalysisService/FileAnalysisService/ FileAnalysisService/FileAnalysisService/

WORKDIR /src/FileAnalysisService/FileAnalysisService
RUN dotnet build FileAnalysisService.csproj -c Release -o /app/build

FROM build AS publish
WORKDIR /src/FileAnalysisService/FileAnalysisService
RUN dotnet publish FileAnalysisService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileAnalysisService.dll"]
