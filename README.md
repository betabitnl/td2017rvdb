# Rick van den Bosch @ TechDays 2017
Code and supporting files for the TechDays 2017 session of @rickvdbosch
"Going Serverless (2/2): Hands-on with Azure Event Grid"

The slides can be found @ https://www.slideshare.net/rickvdbosch/techdays-2017-going-serverless-22-handson-with-azure-event-grid

The list of To-Do's:

# Setting up BlobStorage for Event Grid
1. Enable Blob Storage events - Preview
`az feature register --name storageEventSubscriptions --namespace Microsoft.EventGrid`
2. Create a Storage account _westcentralus only, blobstorage only_
`az storage account create --name <NAME_OF_STORAGE_ACCOUNT> --location westcentralus --resource-group <NAME_OF_RESOURCEGROUP> --sku Standard_LRS --kind storage`
3. Check approval status
`az feature show --name storageEventSubscriptions --namespace Microsoft.EventGrid --query properties.state`
