# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
WORKDIR /app
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
RUN apt-get update && apt-get install -y poppler-utils tzdata
ENV TZ=Europe/Amsterdam

WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "UvA.DataNose.ScanTool.dll"]
