param(
    [string]$Configuration = "Release",
    [string]$Platform = "x64",
    [string]$PackageProject = ".\LockScreen.Package\LockScreen.Package.wapproj",
    [string]$OutputDir = ".\artifacts\msix\"
)

$ErrorActionPreference = "Stop"

function Resolve-VsWhere {
    $default = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
    if (Test-Path $default) { return $default }

    $fallback = Get-ChildItem "${env:ProgramFiles(x86)}\Microsoft Visual Studio" -Recurse -Filter vswhere.exe -ErrorAction SilentlyContinue |
        Select-Object -First 1 -ExpandProperty FullName
    return $fallback
}

function Resolve-MSBuild {
    $vswhere = Resolve-VsWhere
    if ($vswhere) {
        $msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
        if ($msbuild) {
            return $msbuild
        }
    }

    $fallbackCandidates = @(
        "C:\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
    )

    foreach ($candidate in $fallbackCandidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    throw "MSBuild.exe not found. Run .\install-msix-buildtools.ps1 first."
}

$resolvedPackageProject = Resolve-Path $PackageProject
$resolvedOutput = Resolve-Path (New-Item -ItemType Directory -Force -Path $OutputDir)
$msbuildPath = Resolve-MSBuild

Write-Host "Building MSIX package..." -ForegroundColor Cyan
Write-Host "MSBuild: $msbuildPath"
Write-Host "Project: $resolvedPackageProject"
Write-Host "Output: $resolvedOutput"

& $msbuildPath $resolvedPackageProject `
    /restore `
    /p:Configuration=$Configuration `
    /p:Platform=$Platform `
    /p:UapAppxPackageBuildMode=StoreUpload `
    /p:AppxBundle=Always `
    /p:AppxBundlePlatforms=$Platform `
    /p:AppxPackageSigningEnabled=false `
    /p:AppxPackageDir="$resolvedOutput\"

if ($LASTEXITCODE -ne 0) {
    throw "MSIX build failed with exit code $LASTEXITCODE."
}

Write-Host ""
Write-Host "MSIX build completed." -ForegroundColor Green
Write-Host "Check output under: $resolvedOutput"
Write-Host "Expected file type: *.msixupload"
