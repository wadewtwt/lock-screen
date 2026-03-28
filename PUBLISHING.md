# Publishing

## One-click single EXE publish

Run:

```powershell
.\publish-single.ps1
```

Default behavior:

- `Release`
- `win-x64`
- self-contained
- single-file

Output directory:

```text
artifacts\single-file\win-x64\
```

## Common variants

Publish for `win-arm64`:

```powershell
.\publish-single.ps1 -Runtime win-arm64
```

Custom output folder:

```powershell
.\publish-single.ps1 -OutputRoot .\dist
```

## What the script does

It runs:

```powershell
dotnet publish .\LockScreen.App -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

with a few extra publish options to improve the single-file result.

## Notes

- The generated EXE is intended for direct distribution.
- Because this is self-contained, target machines do not need to preinstall the .NET 8 Desktop Runtime.
- File size will be much larger than a framework-dependent publish.
