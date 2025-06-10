param(
    [Parameter(Mandatory=$true)]
    [string]
    $EnvironmentName,
    [Parameter(Mandatory=$true)]
    [string]
    $Location
)

Write-Output "Provisioning environment $EnvironmentName"
$result = az deployment sub create --name "Provision-$(Get-Random)" --template-file $PSScriptRoot/main.bicep --location $Location --parameters environmentName="$EnvironmentName" location="$Location" | ConvertFrom-Json

Write-Output "OK"
Write-Output ""

$sentinelWorkspaceName = $result.properties.outputs.sentinelWorkspaceName.value
$rgName = $result.properties.outputs.rgName.value

Write-Output "Provisioned sentinel workspace $sentinelWorkspaceName"
Write-Output ""

Write-Output "When finished, run:"
Write-Output "az group delete --name $rgName"
