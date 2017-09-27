#!/bin/bash

# -> dipsbot.azurecr.io
docker tag dipsbot dipsbot.azurecr.io/dipsbot
docker push dipsbot.azurecr.io/dipsbot

az container delete --name dipsbot --resource-group kitchen-responsible-rg

az acr credential show --name dipsbot --query "passwords[0].value"
az container create --name dipsbot --image dipsbot.azurecr.io/dipsbot --cpu 1 --memory 1 --registry-password [PASSWORD] --ip-address public -g kitchen-responsible-rg