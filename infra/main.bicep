//
// Deploys a complete set of needed resources for the demo
//
// Includes:
//    * Sentinel-enabled Log Analytics Workspace
//

targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention')
param environmentName string

@minLength(1)
@description('Primary location for all resources')
param location string 

@description('Unique suffix for all resources in this deployment')
@minLength(5)
param suffix string = uniqueString(subscription().id,environmentName)

// Deploy a resource group

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
}

// Deploy resources

module resources 'resources.bicep' = {
  name: 'resources'
  scope: rg
  params: {
    suffix: suffix
    location: location
  }
}

output sentinelWorkspaceName string = resources.outputs.sentinelWorkspaceName
output rgName string = rg.name
