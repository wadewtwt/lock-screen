param(
    [string]$InstallerPath = ".\tools\vs_BuildTools.exe",
    [string]$InstallPath = "C:\BuildTools"
)

$ErrorActionPreference = "Stop"

$installerDir = Split-Path -Parent $InstallerPath
if (-not (Test-Path $installerDir)) {
    New-Item -ItemType Directory -Path $installerDir | Out-Null
}

if (-not (Test-Path $InstallerPath)) {
    Write-Host "Downloading Visual Studio Build Tools bootstrapper..." -ForegroundColor Cyan
    Invoke-WebRequest -Uri "https://aka.ms/vs/17/release/vs_BuildTools.exe" -OutFile $InstallerPath
}

Write-Host "Installing MSIX build toolchain (this may take several minutes)..." -ForegroundColor Cyan

$arguments = @(
    "--quiet",
    "--wait",
    "--norestart",
    "--nocache",
    "--installPath", "`"$InstallPath`"",
    "--add", "Microsoft.VisualStudio.Workload.ManagedDesktopBuildTools",
    "--add", "Microsoft.VisualStudio.Workload.UniversalBuildTools",
    "--add", "Microsoft.VisualStudio.ComponentGroup.MSIX.Packaging",
    "--add", "Microsoft.VisualStudio.Component.Windows10SDK.19041",
    "--includeRecommended"
)

& $InstallerPath $arguments

if ($LASTEXITCODE -ne 0) {
    throw "Build tools install failed with exit code $LASTEXITCODE."
}

Write-Host ""
Write-Host "Build tools install finished." -ForegroundColor Green
Write-Host "Install path: $InstallPath"
Write-Host "Next step: run .\\build-msix.ps1"
