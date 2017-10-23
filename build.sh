#!/bin/bash

rm -r ./publish
set -e
dotnet publish src/Hjerpbakk.DIPSBot.Runner/Hjerpbakk.DIPSBot.Runner.csproj -o ../../publish -c Release
docker build -t dipsbot .

