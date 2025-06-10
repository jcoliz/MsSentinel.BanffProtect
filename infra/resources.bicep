//
// Deploys a complete set of needed resources for the sample at:
//    https://github.com/jcoliz/MsSentinel.HandsOnLab.1
//
// Includes:
//    * Sentinel-enabled Log Analytics Workspace
//    * Azure container app running a synthetic API endpoint
//

@minLength(1)
@description('Primary location for all resources')
param location string = resourceGroup().location

@description('Unique suffix for all resources in this deployment')
@minLength(5)
param suffix string = uniqueString(resourceGroup().id)

// Deploy Microsoft Sentinel Workspace

module workspace 'AzDeploy.Bicep/SecurityInsights/sentinel-complete.bicep' = {
  name: 'workspace'
  params: {
    suffix: suffix
    location: location
  }
}

output sentinelWorkspaceName string = workspace.outputs.logAnalyticsName
