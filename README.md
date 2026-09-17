# Smart Parking Prototype

Windows Forms parking-selection prototype with vehicle, location, slot and reservation-summary screens.

## Overview

Desktop parking-selection UI with vehicle/location selection, a parking map, slot selection and reservation-success screens.

## Features

Custom-painted rounded controls, screen navigation, hover/click state, dynamic reservation display.

## Tech Stack

C# /  .NET / WinForms

## Getting Started

Requires Windows and the .NET 8 SDK with Windows Forms support. Open `SmartParking.sln`, which references `SmartParking/SmartParking.csproj`, or run `dotnet run --project SmartParking/SmartParking.csproj`. The solution includes older Form1 files excluded by the canonical project configuration; those source files remain unchanged. Implemented scope is UI selection and reservation summaries; sensors, persistent bookings, QR and payment integrations are absent.

## Validation

Lightweight local checks: .NET 9 SDK build targeting .NET 8 Windows Forms. Compilation and syntax checks do not verify application behavior. Source is preserved; interactive application behavior was not executed during archival.

## Notes

Original implementation is preserved. Build outputs, dependencies, machine-specific IDE state, backups, submission documents and private runtime data are excluded. No license has been inferred for the original work.
