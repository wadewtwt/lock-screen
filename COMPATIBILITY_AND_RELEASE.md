# Compatibility And Release

## Supported Windows

Current project stack:

- `WPF`
- `.NET 8`
- `TargetFramework: net8.0-windows`

Recommended minimum supported system:

- `Windows 10 22H2`
- `Windows 11 22H2 / 23H2 / 24H2`

Not recommended as official support targets:

- `Windows 7`
- `Windows 8 / 8.1`
- `Windows 10 21H2` and earlier

## Why

This project is built on `.NET 8` and WPF. In practice, the safest external statement is:

`Supports Windows 10 22H2 and later`

That gives us a cleaner support boundary for:

- multi-monitor behavior
- DPI scaling
- newer desktop runtime behavior
- future maintenance

## Current Project Config

Key config is in `LockScreen.App/LockScreen.App.csproj`:

```xml
<TargetFramework>net8.0-windows</TargetFramework>
<UseWPF>true</UseWPF>
<UseWindowsForms>true</UseWindowsForms>
```

`UseWindowsForms` is enabled because the app uses Windows screen enumeration for multi-monitor lock windows.

## Recommended Release Statement

You can use this wording directly:

`This application supports Windows 10 22H2 and later, including Windows 11.`

If you want a Chinese version:

`本应用当前支持 Windows 10 22H2 及以上版本，包括 Windows 11。`

## Local Run

```powershell
dotnet build .\LockScreen.sln
dotnet run --project .\LockScreen.App
```

## Local Publish

Framework-dependent publish:

```powershell
dotnet publish .\LockScreen.App -c Release -r win-x64 --self-contained false
```

Output directory:

```text
LockScreen.App\bin\Release\net8.0-windows\win-x64\publish\
```

## Recommended First Release

For your first internal testing build, I recommend:

- target: `win-x64`
- mode: `Release`
- publish type: `framework-dependent`

Reason:

- output is smaller
- easier to iterate
- easier to debug issues on your own machine

## If You Want A Single Machine-Friendly Package Later

You can later switch to self-contained publish:

```powershell
dotnet publish .\LockScreen.App -c Release -r win-x64 --self-contained true
```

This is more convenient for test machines that do not already have the .NET desktop runtime installed, but the package will be much larger.

## Runtime Requirement

If you publish as `framework-dependent`, the target machine needs:

- `.NET 8 Desktop Runtime`

If you publish as `self-contained`, you do not need to preinstall the runtime on the target machine.

## If You Need Older Windows Support

Possible downgrade paths:

- for older Windows 10 compatibility: consider `net6.0-windows`
- for Windows 7 or 8.1 style compatibility goals: usually move to `.NET Framework 4.8 WPF`

That would be a deliberate compatibility tradeoff and would increase migration cost, so it is not recommended unless you truly need old-system support.
