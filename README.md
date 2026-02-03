# RDP Launcher

A modern Windows 11 application for launching Remote Desktop Protocol (RDP) connections with precise control over resolution and monitor positioning.

## Overview

RDP Launcher provides an elegant, lightweight interface for managing RDP connections. It's particularly useful for working around scaling issues with HiDPI (4K+) monitors by allowing you to specify exact resolutions and window positions.

## Features

- 🎨 **Modern WinUI 3 Interface** - Sleek Windows 11 design with Mica background effect
- 📐 **Custom Resolution Control** - Set precise width and height for RDP sessions
- ⚡ **Quick Presets** - One-click buttons for common resolutions (1080p, 1440p, 4K)
- 🖥️ **Multi-Monitor Support** - Position RDP windows on specific monitors
- 💾 **Position Memory** - Automatically remembers launcher window position
- 🪶 **Lightweight** - Minimal design focused on core functionality

## Quick Start

### Prerequisites

- Windows 11 (recommended) or Windows 10 1809+
- .NET 8.0 Runtime
- Visual Studio 2022 (for building from source)

### Building

```powershell
# Clone the repository
git clone https://github.com/BW-PA/RDP-Launcher.git
cd RDP-Launcher

# Restore packages and build
dotnet restore
dotnet build -c Release

# Run the application
dotnet run --project RdpLauncher/RdpLauncher.csproj
```

### Usage

1. Enter the server address (hostname or IP)
2. Set your desired resolution (or use a preset)
3. Choose the target monitor position
4. Click "Launch RDP Connection"

The launcher stays open for subsequent connections and remembers its window position between sessions.

## Architecture

- **Framework**: .NET 8.0 with Windows App SDK
- **UI**: WinUI 3 (Windows 11 native)
- **Backdrop**: Mica material effect
- **Persistence**: Local settings in `%LOCALAPPDATA%\RdpLauncher`

## Documentation

See [DEVELOPMENT.md](DEVELOPMENT.md) for detailed development guidelines, architecture details, and contribution information.

## Reference Implementation

The `reference/` folder contains the original PowerShell script that demonstrates the core RDP launching functionality. The WinUI 3 app provides the same capabilities with a modern user interface.

## License

See LICENSE file for details.
