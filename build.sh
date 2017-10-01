#!/bin/bash

rm -r ./publish
set -e
dotnet restore
dotnet build
dotnet publish src/Hjerpbakk.DIPSBot.Runner/Hjerpbakk.DIPSBot.Runner.csproj -o ../../publish -c Release
docker build -t dipsbot .

