# Wake Resume Setup

## What this does

This project can be configured to start the lock screen app automatically when Windows resumes from sleep.

More precisely:

- it does not run while the PC is sleeping
- it launches when the PC wakes up and resumes into Windows

## Prerequisite

Build the single-file EXE first:

```powershell
.\publish-single.ps1
```

Expected EXE:

```text
artifacts\single-file\win-x64\LockScreen.App.exe
```

## Register the wake task

Run:

```powershell
.\register-resume-lockscreen.ps1
```

This creates a Windows Scheduled Task triggered by:

- `Microsoft-Windows-Power-Troubleshooter`
- `Event ID 1`

That event is commonly raised when Windows resumes from sleep.

## Remove the wake task

Run:

```powershell
.\unregister-resume-lockscreen.ps1
```

## Custom EXE path

If your EXE is in a different location:

```powershell
.\register-resume-lockscreen.ps1 -ExePath "D:\apps\LockScreen.App.exe"
```

## Notes

- This is a system integration step, not an app-only setting.
- The EXE should stay at a stable path after task registration.
- If you move or rename the EXE, re-register the task.
