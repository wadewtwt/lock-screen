# MSIX Setup

## Goal

This repository now includes a dedicated MSIX packaging project:

- app project: `LockScreen.App`
- packaging project: `LockScreen.Package`

Use this structure when preparing the app for Microsoft Store submission.

## Recommended format

Recommended Store packaging format:

- `MSIX`

Typical Store upload output:

- `msixupload`

## Current package project

Packaging project file:

- `LockScreen.Package\LockScreen.Package.wapproj`

Manifest:

- `LockScreen.Package\Package.appxmanifest`

Assets:

- `LockScreen.Package\Assets\`

## Before first Store submission

Replace placeholder metadata in the manifest:

- package identity
- publisher
- publisher display name
- logos if you want production-quality branding

## Visual Studio workflow

1. Open the solution in Visual Studio 2022
2. Make sure the Windows app packaging tooling is installed
3. Open the packaging project
4. Associate the app with your Store listing
5. Build the packaging project in `Release | x64`
6. Generate the Store upload package

## Why not EXE for Store

Your single-file EXE is still useful for private distribution and internal testing, but Microsoft Store submission should use the packaging project and MSIX flow.

## Current assumptions

- target architecture: `x64`
- app minimum target: `Windows 10 1809+`
- recommended real-world support statement: `Windows 10 22H2 and later`
