param(
    [string]$TaskName = "LockScreen On Resume"
)

$ErrorActionPreference = "Stop"

if (Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue) {
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false
    Write-Host "Scheduled task removed: $TaskName" -ForegroundColor Green
}
else {
    Write-Host "Scheduled task not found: $TaskName" -ForegroundColor Yellow
}
