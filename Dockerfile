FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app

RUN apt-get update && apt-get install -y poppler-utils

COPY ./bin/Release/net5.0/publish ./

ENTRYPOINT ["dotnet", "UvA.DataNose.ScanTool.dll"]