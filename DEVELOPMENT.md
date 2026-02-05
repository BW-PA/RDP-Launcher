# RDP Launcher - Development Guide

## Overview

RDP Launcher is a modern Windows 11 application built with WinUI 3 that provides an elegant interface for launching Remote Desktop Protocol (RDP) connections with precise control over resolution and monitor positioning. This is particularly useful for working around scaling issues with HiDPI (4K+) monitors.

## Features

### Current Implementation

- **Modern WinUI 3 Interface**: Sleek, Windows 11-native UI with Mica background effect
- **Custom Resolution Control**: Set precise width and height for RDP sessions
- **Quick Resolution Presets**: One-click buttons for common resolutions (1920×1080, 2560×1440, 3840×2160)
- **Monitor Positioning**: Hard-coded monitor position options for multi-monitor setups
- **Window Position Memory**: Automatically remembers and restores launcher window position
- **Lightweight Design**: Minimal bloat, focused on core functionality
- **Stays Open**: Launcher remains open after launching RDP connections

### Technical Stack

- **Framework**: .NET 8.0 with Windows App SDK
- **UI Framework**: WinUI 3 (Windows 11)
- **Backdrop**: Mica effect for modern Windows 11 aesthetic
- **Target Platform**: Windows 10.0.22621.0 (Windows 11)
- **Minimum Platform**: Windows 10.0.17763.0 (Windows 10 1809)

## Project Structure

```
RDP-Launcher/
├── reference/                      # Reference PowerShell implementation
│   ├── launch-rdp.ps1             # Original PowerShell script
│   └── README.md                  # Reference documentation
├── RdpLauncher/                   # WinUI 3 application
│   ├── Assets/                    # App assets and icons
│   ├── App.xaml                   # Application definition
│   ├── App.xaml.cs                # Application code-behind
│   ├── MainWindow.xaml            # Main window UI
│   ├── MainWindow.xaml.cs         # Main window logic
│   ├── RdpLauncher.csproj        # Project file
│   └── app.manifest              # Application manifest
├── RdpLauncher.sln               # Visual Studio solution
├── .gitignore                    # Git ignore rules
└── README.md                     # Project readme
```

## Building the Application

### Prerequisites

- Windows 11 (recommended) or Windows 10 1809+
- Visual Studio 2022 (17.0 or later) with:
  - .NET Desktop Development workload
  - Windows App SDK
  - WinUI 3 templates
- .NET 8.0 SDK

### Build Steps

1. **Open the solution**
   ```
   Open RdpLauncher.sln in Visual Studio 2022
   ```

2. **Restore NuGet packages**
   ```
   Right-click solution → Restore NuGet Packages
   ```

3. **Select platform**
   - Choose x64, x86, or ARM64 from the platform dropdown

4. **Build**
   ```
   Build → Build Solution (Ctrl+Shift+B)
   ```

5. **Run**
   ```
   Debug → Start Without Debugging (Ctrl+F5)
   ```

### Command Line Build

```powershell
# Navigate to solution directory
cd /path/to/RDP-Launcher

# Restore packages
dotnet restore

# Build for x64
dotnet build -c Release -r win-x64

# Run
dotnet run --project RdpLauncher/RdpLauncher.csproj
```

## Usage

### Launching an RDP Connection

1. **Enter Server Address**: Type the hostname or IP address of the remote server
2. **Set Resolution**: 
   - Use the Width/Height number boxes for precise control
   - Or click a preset button (1920×1080, 2560×1440, 3840×2160)
3. **Choose Monitor Position**: Select from the dropdown for multi-monitor setups
   - Primary Monitor (0, 0)
   - Secondary Monitor Left (-1920, 0)
   - Secondary Monitor Right (1920, 0)
   - Secondary Monitor Above (0, -1080)
   - Secondary Monitor Below (0, 1080)
4. **Launch**: Click "Launch RDP Connection" button

### Window Position Persistence

The application automatically saves its window position when closed and restores it on next launch. Settings are stored in:
```
%LOCALAPPDATA%\RdpLauncher\rdp-launcher-settings.txt
```

## Architecture Details

### Mica Backdrop

The application uses Windows 11's Mica material for a modern, semi-transparent background that adapts to the system theme:

```csharp
if (MicaController.IsSupported())
{
    m_backdropController = new MicaBackdrop();
    m_backdropController.Kind = MicaKind.Base;
    this.SystemBackdrop = m_backdropController;
}
```

### RDP File Generation

The app generates temporary .rdp files with precise settings for resolution and positioning:

- Configures window position using `winposstr` parameter
- Sets desktop dimensions with `desktopwidth` and `desktopheight`
- Includes optimal settings for performance and quality
- Cleans up temporary files after launch

### Monitor Positioning

Currently uses hard-coded monitor positions (as per requirements). The positions are stored as X,Y coordinates:
- Primary: (0, 0)
- Left: (-1920, 0)
- Right: (1920, 0)
- Above: (0, -1080)
- Below: (0, 1080)

Future versions may include automatic monitor detection.

## Development Guidelines

### IDE

This project is designed to work with **Visual Studio Code** as well as Visual Studio 2022. For VS Code:

1. Install C# Dev Kit extension
2. Install Windows App SDK extension (if available)
3. Use integrated terminal for `dotnet` commands

### AI-First Development

This project was built as an AI coding exercise, leveraging:
- GitHub Copilot for code suggestions
- Copilot CLI for command-line assistance
- AI-generated initial structure and implementation

### Design Principles

1. **Lightweight**: Minimal dependencies, fast startup
2. **Modern**: Windows 11 design language with Mica backdrop
3. **Focused**: Core functionality without bloat
4. **User-Friendly**: Clear, intuitive interface
5. **Persistent**: Remembers user preferences

## Future Enhancements

Potential improvements for future versions:

- [ ] Automatic monitor detection and enumeration
- [ ] Save/load connection profiles
- [ ] Recent connections list
- [ ] Custom RDP settings configuration
- [ ] Keyboard shortcuts
- [ ] System tray integration
- [ ] Multiple connection presets
- [ ] Export/import settings

## Troubleshooting

### Build Issues

**Issue**: NuGet packages not restoring
**Solution**: 
```powershell
dotnet nuget locals all --clear
dotnet restore --force
```

**Issue**: Windows App SDK not found
**Solution**: Install via Visual Studio Installer or download from Microsoft

### Runtime Issues

**Issue**: Mica backdrop not appearing
**Solution**: Ensure running on Windows 11. On Windows 10, the app will fall back to standard window background.

**Issue**: RDP not launching
**Solution**: Verify `mstsc.exe` is available in system PATH

## License

See LICENSE file for details.

## Contributing

This project welcomes contributions. Please ensure:
- Code follows existing patterns
- UI maintains modern, lightweight aesthetic
- Changes are tested on Windows 11
- Documentation is updated accordingly

## Support

For issues and questions, please use the GitHub issue tracker.
