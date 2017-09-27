#!/bin/bash

rm -r ./publish
dotnet restore
dotnet build
dotnet publish src/Hjerpbakk.DIPSBot.Runner/Hjerpbakk.DIPSBot.Runner.csproj -o ../../publish -c Release
docker build -t dipsbot .

# -> dipsbot.azurecr.io
docker tag dipsbot dipsbot.azurecr.io/dipsbot
docker push dipsbot.azurecr.io/dipsbot

az acr credential show --name dipsbot --query "passwords[0].value"
az container create --name dipsbot --image dipsbot.azurecr.io/dipsbot --cpu 1 --memory 1 --registry-password [PASSWORD] --ip-address public -g kitchen-responsible-rg