# MsSentinel.BanffProtect Demo

[![Build+Test](https://github.com/jcoliz/MsSentinel.BanffProtect/actions/workflows/build+test.yaml/badge.svg)](https://github.com/jcoliz/MsSentinel.BanffProtect/actions/workflows/build+test.yaml)

This demo mocks the Jamf Protect admin console, demonstrating how to set up the Jamf Protect connector in Microsoft Sentinel.

## Prerequisites

To bring up this demo, you will first need:

* An Azure account. Set up a [Free Azure Account](https://azure.microsoft.com/en-us/pricing/purchase-options/azure-account) to get started.
* [Azure CLI tool with Bicep](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#azure-cli)
* A git client, e.g. [Git for Windows](https://gitforwindows.org/)
* Execution policy configured to run unsigned PowerShell scripts, see [About Execution Policies](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_execution_policies)
* [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) SDK
* [.NET Aspire Workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=windows&pivots=vscode). Note that no container runtime is needed for this demo.

## Provision Azure resources

For this demo, it's helpful to have a dedicated Sentinel Workspace. To provision a dedicated workspace, run the [Provision-Resources.ps1](./infro/Provision-Resources.ps1) script. Supply a memorable environment name and Azure datacenter location according to your preference. These instructions imagine you've used `banff-protect` for this. If you've chosen something different, be sure to
use the correct moniker where needed.

```dotnetcli
.\infra\Provision-Resources.ps1 -EnvironmentName banff-protect -Location westus
```

When this script completes, it will pass along some helpful information:

```dotnetcli
Deployed sentinel workspace sentinel-something

When finished, run:
az group delete --name rg-banff-protect
```
