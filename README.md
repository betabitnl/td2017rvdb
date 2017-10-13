# Rick van den Bosch @ TechDays 2017
Code and supporting files for the TechDays 2017 session of @rickvdbosch
"Going Serverless (2/2): Hands-on with Azure Event Grid"

The slides can be found @ https://www.slideshare.net/rickvdbosch/techdays-2017-going-serverless-22-handson-with-azure-event-grid

The list of To-Do's:

## Setting up BlobStorage for Event Grid
1. Enable Blob Storage events - Preview  
`az feature register --name storageEventSubscriptions --namespace Microsoft.EventGrid`
2. Create a Storage account _westcentralus only, blobstorage only_  
`az storage account create --name <NAME_OF_STORAGE_ACCOUNT> --location westcentralus --resource-group <NAME_OF_RESOURCEGROUP> --sku Standard_LRS --kind storage`
3. Check approval status  
`az feature show --name storageEventSubscriptions --namespace Microsoft.EventGrid --query properties.state`

## Logic App
Configuring the Logic App to trigger on an Event Grid event requires information like  
- Resource type (custom, _Microsoft.Storage_)  
- Resource name (custom, use http://resources.azure.com)  
- Prefix filter (optional, use _/blobServices/default/containers/mycontainer/_) to filter to the _mycontainer_ container  

## WebHook
Please keep [RequestBin](https://requestb.in/) in mind, the website that shows all requests you send to your temporary URL.
1. Create an Event Grid Subscription (WebHook)
2. Configure the endpoint URL in the WebHook subscription
3. Upload or delete a Blob in your Blob Storage
4. Refresh the Requestbin and seen what happened :)

## Creating and calling a Custom Topic
1. Create a Resource Group  
`az group create --name <RESOURCEGROUP_NAME> --location westcentralus`
2. Create a custom topic  
`az eventgrid topic create --name <TOPIC_NAME> -l westcentralus -g <RESOURCEGROUP_NAME>`
3. Create a message endpoint (like Requestbin!)  
https://requestb.in
4. Subscribe to a topic  
`az eventgrid topic event-subscription create --name <SUBSCRIPTION_NAME> --endpoint <REQUESTBIN_URL> -g <RESOURCEGROUP_NAME> --topic-name <TOPIC_NAME>`
5. Send an event to your topic   
* `endpoint=$(az eventgrid topic show --name <TOPIC_NAME> -g <RESOURCEGROUP_NAME> --query "endpoint" --output tsv)`  
* `key=$(az eventgrid topic key list --name <TOPIC_NAME> -g <RESOURCEGROUP_NAME> --query "key1" --output tsv)`  
* `body=$(eval echo "'$(curl https://raw.githubusercontent.com/betabitnl/td2017rvdb/master/Datafiles/customevent.json)'")`  
* `curl -X POST -H "aeg-sas-key: $key" -d "$body" $endpoint`  
6. Clean up resources  
`az group delete --name <RESOURCEGROUP_NAME>`
