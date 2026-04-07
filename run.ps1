# eHesabim.Web.Portal - Build & Run
$msbuild = "C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe"

Write-Host "Building solution..." -ForegroundColor Cyan
& $msbuild "$PSScriptRoot\eHesabim.sln" /p:Configuration=Debug /t:Build /m /v:minimal
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build FAILED!" -ForegroundColor Red
    exit 1
}

Write-Host "`nStarting IIS Express on http://localhost:16347 ..." -ForegroundColor Green
& "C:\Program Files\IIS Express\iisexpress.exe" /path:"$PSScriptRoot\eHesabim.Web.Portal" /port:16347
