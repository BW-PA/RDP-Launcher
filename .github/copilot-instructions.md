# Copilot Instructions for RDP Launcher

## Project Overview

RDP Launcher is a WinUI 3 desktop application for Windows that launches Remote Desktop Protocol (RDP) connections with precise resolution and monitor positioning control. It addresses HiDPI/4K monitor scaling issues.

**Technology Stack:**
- .NET 8.0 with Windows App SDK 1.5
- WinUI 3 (Windows 11 native UI)
- Target: Windows 10.0.22621.0 (Windows 11), Minimum: 10.0.17763.0 (Windows 10 1809)

## Build & Test Commands

### Building
```powershell
# Restore NuGet packages
dotnet restore

# Build for x64 platform (default)
dotnet build -c Release -r win-x64

# Build for Debug configuration
dotnet build -c Debug

# Run the application
dotnet run --project RdpLauncher/RdpLauncher.csproj
```

### Testing
**No automated tests currently exist.** Manual testing is done on Windows. See TESTING.md for comprehensive test checklist.

### Linting
No linter is configured. Follow existing code patterns and C# conventions.

## Architecture

### Project Structure
- `RdpLauncher/` - Main WinUI 3 application project
  - `MainWindow.xaml` - UI layout (~140 lines)
  - `MainWindow.xaml.cs` - Business logic (~280 lines)
  - `App.xaml[.cs]` - Application entry point
  - `RdpLauncher.csproj` - Project file with dependencies
- `reference/launch-rdp.ps1` - Original PowerShell reference implementation
- `src/RdpLauncher/` - Symbolic link to RdpLauncher project (ignore this)

### Application Flow
1. User enters server address, resolution (or preset), and monitor position
2. Validation enables/disables Launch button
3. On launch: generate temporary .rdp file with precise settings
4. Execute `mstsc.exe` with the .rdp file
5. Display success message, clean up .rdp file after 2 seconds
6. Window stays open for additional connections

### Key Components

**Mica Backdrop**: Windows 11 translucent material effect applied via `MicaController`. Gracefully degrades on Windows 10.

**RDP File Generation**: Creates temporary .rdp files with:
- `winposstr` for exact window positioning (X, Y, Width, Height)
- `desktopwidth` and `desktopheight` for resolution
- Performance/quality optimizations

**Settings Persistence**: Window position saved to `%LOCALAPPDATA%\RdpLauncher\rdp-launcher-settings.txt` with simple key=value format:
```
WindowX=100
WindowY=200
```

**Monitor Positioning**: Currently uses hard-coded positions (by design):
- Primary Monitor: (0, 0)
- Secondary Left: (-1920, 0)
- Secondary Right: (1920, 0)
- Secondary Above: (0, -1080)
- Secondary Below: (0, 1080)

## Conventions & Patterns

### File Organization
- XAML files define UI structure declaratively
- Code-behind (.cs) files contain event handlers and business logic
- No MVVM pattern used - simple code-behind architecture for lightweight app

### Naming Conventions
- UI element names: PascalCase with descriptive suffixes (e.g., `ServerAddressTextBox`, `LaunchButton`)
- Private fields: camelCase with `m_` prefix for backing fields (e.g., `m_backdropController`)
- Event handlers: `ElementName_EventName` pattern (e.g., `LaunchButton_Click`)

### Code Style
- Uses C# 10+ features (file-scoped namespaces, pattern matching)
- WinUI 3 API patterns for window management (`AppWindow`, `WindowNative`, `Win32Interop`)
- Minimal error handling - focuses on happy path (per lightweight design principle)
- Comments only where code behavior is non-obvious

### XAML Patterns
- Grid-based layouts with RowDefinitions
- StackPanel with Spacing property for vertical layouts
- NumberBox controls for numeric input (Width/Height)
- InfoBar for user feedback messages
- Custom title bar with ExtendedContentIntoTitleBar pattern

### Windows-Specific APIs
- `Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)` for settings path
- `Process.Start()` with `UseShellExecute = true` to launch mstsc.exe
- `WindowNative.GetWindowHandle()` and `Win32Interop` for AppWindow access
- Mica backdrop via `MicaController.IsSupported()` check

## Important Notes

### Platform Constraints
- **Windows-only**: Cannot build or run on Linux/macOS due to WinUI 3 dependency
- **Visual Studio recommended**: While .NET CLI works, WinUI 3 development is optimized for VS 2022
- **Windows SDK required**: Project requires Windows SDK 10.0.22621.0+

### Design Principles
1. **Lightweight**: Minimal dependencies, fast startup (~2s), low memory (~40MB)
2. **Modern**: Windows 11 design language, Mica backdrop, native controls
3. **Focused**: Core RDP launching only, no profile management or advanced features
4. **Persistent**: Remembers window position across sessions

### Known Limitations
- Monitor positions are hard-coded (no automatic detection)
- No connection history or saved profiles
- No custom RDP settings beyond resolution/position
- Temporary .rdp files briefly exist in %TEMP%

### Reference Implementation
The `reference/launch-rdp.ps1` script demonstrates the core RDP launching logic. When changing RDP file generation, ensure behavior matches the PowerShell script's output.

## Development Workflow

### Making Changes
1. Edit XAML for UI changes
2. Edit MainWindow.xaml.cs for logic changes
3. Build and run to test (requires Windows)
4. Update documentation if behavior changes

### Adding Features
- Keep design lightweight and focused
- Follow existing patterns (no MVVM framework, simple code-behind)
- Test on Windows 11 and verify Windows 10 compatibility
- Update TESTING.md checklist for new features

### Dependencies
NuGet packages are pinned in RdpLauncher.csproj:
- Microsoft.WindowsAppSDK 1.5.240802000
- Microsoft.Windows.SDK.BuildTools 10.0.22621.3233
- Microsoft.Graphics.Win2D 1.2.0

Don't update WindowsAppSDK without testing Mica backdrop compatibility.

## Documentation

- **README.md** - Project overview, quick start
- **DEVELOPMENT.md** - Detailed architecture, troubleshooting
- **PROJECT-STRUCTURE.md** - Complete file breakdown, diagrams
- **TESTING.md** - Manual test procedures, checklist
- **IMPLEMENTATION.md** - Design decisions and implementation details
- **QUICKSTART-WINDOWS.md** - Windows-specific setup guide
