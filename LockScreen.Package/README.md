# LockScreen Package

This folder contains the Microsoft Store packaging project for the WPF app.

## What to update before Store submission

Open `LockScreen.Package.wapproj` and `Package.appxmanifest` in Visual Studio, then update:

- package identity
- publisher
- display name
- description
- store association
- final app icons

Important placeholders currently in use:

- `YourCompany.LockScreen`
- `CN=YourCompany`
- `LockScreen`

## Expected workflow

1. Open the solution in Visual Studio 2022
2. Install the Windows app packaging workload if needed
3. Associate the packaging project with your Microsoft Store app
4. Generate an MSIX / MSIXUPLOAD package from the packaging project

## Notes

- This packaging project is intended for Microsoft Store submission.
- Keep using the single-file EXE flow for quick internal distribution.
