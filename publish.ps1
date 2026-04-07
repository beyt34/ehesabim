# eHesabim.Web.Portal - Publish (Release)
param(
    [string]$PublishDir = "C:\Publish\ehesabim.com"
)

$msbuild = "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe"

Write-Host "Publishing to $PublishDir ..." -ForegroundColor Cyan
& $msbuild "$PSScriptRoot\eHesabim.Web.Portal\eHesabim.Web.Portal.csproj" `
    /p:Configuration=Release `
    /p:DeployOnBuild=true `
    /p:PublishProfile=FileSystem `
    /p:publishUrl=$PublishDir `
    /p:DeleteExistingFiles=true `
    /m /v:minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish FAILED!" -ForegroundColor Red
    exit 1
}

Write-Host "`nPublished to: $PublishDir" -ForegroundColor Green
