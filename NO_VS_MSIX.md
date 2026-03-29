# Build MSIX Without Full Visual Studio

You can generate Store-ready package artifacts without installing the full Visual Studio IDE.

## 1. Install build toolchain

Run:

```powershell
.\install-msix-buildtools.ps1
```

This installs:

- MSBuild
- Desktop packaging build tools
- MSIX packaging components

## 2. Build Store upload package

Run:

```powershell
.\build-msix.ps1
```

Default output:

```text
artifacts\msix\
```

Look for:

- `*.msixupload`

## Notes

- This flow does not require the full Visual Studio UI.
- You still need valid Store identity metadata before final submission.
- In `LockScreen.Package\Package.appxmanifest`, replace placeholder values:
  - `Name="YourCompany.LockScreen"`
  - `Publisher="CN=YourCompany"`
  - `PublisherDisplayName="YourCompany"`
