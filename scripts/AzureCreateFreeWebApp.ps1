<#
.SYNOPSIS
    Crea un grupo de recursos, un App Service Plan Linux FREE (F1) y una Web App para .NET.

.DESCRIPTION
    Tras ejecutarlo, descarga el perfil de publicación desde Azure Portal para usarlo en GitHub Actions
    (secreto AZURE_PUBLISH_PROFILE) con el workflow azure-webapp-free.yml.

    Si la pila DOTNETCORE|10.0 no existe en tu región, crea la Web App desde el Portal y elige manualmente el runtime.

.PARAMETER ResourceGroup
.PARAMETER Location
.PARAMETER WebAppName   Debe ser único globalmente (*.azurewebsites.net).
.PARAMETER PlanName
#>
param(
    [string]$ResourceGroup = "rg-businesscentral-free",
    [string]$Location = "eastus",
    [Parameter(Mandatory = $true)][string]$WebAppName,
    [string]$PlanName = "asp-businesscentral-free"
)

$ErrorActionPreference = "Stop"

Write-Host "Creando grupo de recursos $ResourceGroup en $Location..."
az group create --name $ResourceGroup --location $Location | Out-Null

Write-Host "Creando App Service Plan Linux FREE..."
az appservice plan create `
    --name $PlanName `
    --resource-group $ResourceGroup `
    --sku FREE `
    --is-linux | Out-Null

Write-Host "Creando Web App Linux (.NET)..."
$runtime = "DOTNETCORE|10.0"
az webapp create `
    --resource-group $ResourceGroup `
    --plan $PlanName `
    --name $WebAppName `
    --runtime $runtime
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Fallo con runtime $runtime. Lista pilas: az webapp list-runtimes --os linux"
    Write-Warning "Crea la Web App desde Portal y elige el stack .NET disponible (p. ej. 8 LTS temporalmente)."
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "Listo: https://$WebAppName.azurewebsites.net"
Write-Host "Portal → Web App → Configuration: ConnectionStrings__DefaultConnection, JwtSettings__*, etc."
Write-Host "Portal → Web App → Get publish profile → GitHub secret AZURE_PUBLISH_PROFILE"
Write-Host "GitHub secret AZURE_WEBAPP_NAME = $WebAppName"
