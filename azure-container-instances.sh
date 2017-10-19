#!/bin/bash
set -e

container_name="dipsbot"
tagged_container_name="dipsbot.azurecr.io/"$container_name
service_name=$container_name"-service"

# Needs: brew install azure-cli
# Push to Azure Container Registry
# az group create --name kitchen-responsible-rg --location westeurope
# az acr create --name dipsbot --resource-group kitchen-responsible-rg --admin-enabled --sku Basic
az acr login --name dipsbot
# az acr list --resource-group kitchen-responsible-rg --query "[].{acrLoginServer:loginServer}" --output table
# -> dipsbot.azurecr.io
docker tag $container_name $tagged_container_name
docker push $tagged_container_name

# Run in Azure Container Instances
# az acr show --name dipsbot --query loginServer
# az acr credential show --name dipsbot --query "passwords[0].value"
az container delete --name $service_name --resource-group kitchen-responsible-rg --yes
az container create --name $service_name --image $tagged_container_name --cpu 1 --memory 1 --registry-password $AZUREPW --ip-address public -g kitchen-responsible-rg

container_status=$(az container show --name $service_name --resource-group kitchen-responsible-rg --query state)
echo $container_status
while [ $container_status != "\"Running\"" ]
do 
    sleep 5
    container_status=$(az container show --name $service_name --resource-group kitchen-responsible-rg --query state)
    echo $container_status
done

# Uploud IP to Blob Storage
# Needs. AZURE_STORAGE_CONNECTION_STRING environment variable
touch ./$service_name.txt
ip=$(az container show --name $service_name --resource-group kitchen-responsible-rg --query ipAddress.ip | tr -d '"')
echo $ip > ./$service_name.txt
az storage blob upload --container-name discovery --file $service_name.txt --name $service_name.txt
curl -X POST -H "Content-Type: application/json" -d "{\"name\":\"$service_name\", \"ip\":\"$ip\"}" -i http://who-am-i.xyz/api/services/
echo $ip
