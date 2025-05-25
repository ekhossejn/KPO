FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY Common/Common/Common.csproj Common/Common/
COPY ApiGateway/ApiGateway/ApiGateway.csproj ApiGateway/ApiGateway/

RUN dotnet restore ApiGateway/ApiGateway/ApiGateway.csproj

COPY Common/Common/ Common/Common/
COPY ApiGateway/ApiGateway/ ApiGateway/ApiGateway/

WORKDIR /src/ApiGateway/ApiGateway
RUN dotnet build ApiGateway.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ApiGateway.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
