#!/bin/bash
set -e
rm -r ./publish
dotnet restore
dotnet build
dotnet publish src/Hjerpbakk.DIPSBot.Runner/Hjerpbakk.DIPSBot.Runner.csproj -o ../../publish -c Release
docker build -t dipsbot .

