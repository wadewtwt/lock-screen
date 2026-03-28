param(
    [string]$Runtime = "win-x64",
    [string]$Configuration = "Release",
    [string]$Project = ".\LockScreen.App",
    [string]$OutputRoot = ".\artifacts\single-file"
)

$ErrorActionPreference = "Stop"

$projectPath = Resolve-Path $Project
$outputDir = Join-Path $OutputRoot $Runtime

Write-Host "Publishing single-file build..." -ForegroundColor Cyan
Write-Host "Project: $projectPath"
Write-Host "Runtime: $Runtime"
Write-Host "Configuration: $Configuration"
Write-Host "Output: $outputDir"

dotnet publish $projectPath `
  -c $Configuration `
  -r $Runtime `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true `
  -o $outputDir

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE."
}

Write-Host ""
Write-Host "Single-file publish completed." -ForegroundColor Green
Write-Host "Output directory: $outputDir"
