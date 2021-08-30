# UvA.DataNose.ScanTool

Monitors a folder for scans and pushes them to the DataNose API.

## Setup

Create a folder to contain the incoming scans and an archive folder. Then run
```sh
docker run -d \
    -v /incoming:/files \
    -v /archive:/archive \
    -e TargetHost=https://url-to-DN-api \
    -e ApiKey=SomeSecret \
    uvafnwi/scantool
```

## Build
Run
```sh
dotnet publish -c Release && docker build -t uvafnwi/scantool . && docker push uvafnwi/scantool
```