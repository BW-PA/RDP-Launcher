# RDP Launcher - Project Structure

## Directory Tree

```
RDP-Launcher/
├── 📄 README.md                          # Project overview and quick start
├── 📄 DEVELOPMENT.md                     # Detailed development guide
├── 📄 IMPLEMENTATION.md                  # Implementation details and decisions
├── 📄 TESTING.md                         # Testing procedures and checklist
├── 📄 .gitignore                         # Git ignore rules
│
├── 📄 RdpLauncher.sln                    # Visual Studio solution
│
├── 📁 RdpLauncher/                       # Main WinUI 3 application
│   ├── 📄 RdpLauncher.csproj            # Project configuration
│   ├── 📄 app.manifest                  # Application manifest (DPI awareness)
│   ├── 📄 App.xaml                      # Application resources and theme
│   ├── 📄 App.xaml.cs                   # Application entry point
│   ├── 📄 MainWindow.xaml               # Main window UI layout
│   ├── 📄 MainWindow.xaml.cs            # Main window logic
│   └── 📁 Assets/                       # Application assets
│       └── 📄 README.md                 # Assets documentation
│
└── 📁 reference/                        # Reference implementation
    ├── 📄 README.md                     # Reference documentation
    └── 📄 launch-rdp.ps1                # Original PowerShell script
```

## File Descriptions

### Root Level Documentation

| File | Purpose | Lines |
|------|---------|-------|
| **README.md** | Project overview, features, quick start guide | ~50 |
| **DEVELOPMENT.md** | Architecture, building, troubleshooting, contributing | ~280 |
| **IMPLEMENTATION.md** | Technical implementation details and decisions | ~350 |
| **TESTING.md** | Testing procedures, checklist, verification | ~240 |

### Solution & Project Files

| File | Purpose |
|------|---------|
| **RdpLauncher.sln** | Visual Studio solution file |
| **RdpLauncher.csproj** | MSBuild project file with NuGet dependencies |
| **app.manifest** | Windows manifest for DPI awareness and compatibility |

### Application Code

| File | Lines | Description |
|------|-------|-------------|
| **App.xaml** | ~20 | Application-level XAML resources |
| **App.xaml.cs** | ~20 | Application lifecycle management |
| **MainWindow.xaml** | ~140 | Complete UI layout with controls |
| **MainWindow.xaml.cs** | ~280 | Business logic, RDP launching, persistence |

### Reference Implementation

| File | Lines | Description |
|------|-------|-------------|
| **launch-rdp.ps1** | ~90 | PowerShell script showing core functionality |

## Technology Stack

```
┌─────────────────────────────────────┐
│       RDP Launcher Application      │
│                                     │
│  ┌───────────────────────────────┐ │
│  │      WinUI 3 UI Layer         │ │
│  │  (MainWindow.xaml)            │ │
│  └───────────┬───────────────────┘ │
│              │                      │
│  ┌───────────▼───────────────────┐ │
│  │   Application Logic Layer     │ │
│  │  (MainWindow.xaml.cs)         │ │
│  │  - Input validation           │ │
│  │  - RDP file generation        │ │
│  │  - Settings persistence       │ │
│  └───────────┬───────────────────┘ │
│              │                      │
│  ┌───────────▼───────────────────┐ │
│  │    Windows App SDK 1.5        │ │
│  │  - Mica backdrop              │ │
│  │  - Modern controls            │ │
│  │  - Windowing APIs             │ │
│  └───────────┬───────────────────┘ │
│              │                      │
│  ┌───────────▼───────────────────┐ │
│  │     .NET 8.0 Runtime          │ │
│  └───────────┬───────────────────┘ │
└──────────────┼─────────────────────┘
               │
    ┌──────────▼──────────┐
    │   Windows 11 OS     │
    │  - mstsc.exe (RDP)  │
    │  - File system      │
    └─────────────────────┘
```

## Key Components

### 1. User Interface (XAML)
```
MainWindow.xaml (140 lines)
├── Title Bar (Custom)
│   ├── App Icon
│   └── Title Text
│
└── Main Content
    ├── Server Address Input
    ├── Resolution Controls
    │   ├── Width NumberBox
    │   ├── Height NumberBox
    │   └── Preset Buttons (1080p, 1440p, 4K)
    ├── Monitor Position ComboBox
    ├── Launch Button
    └── Status InfoBar
```

### 2. Business Logic (C#)
```
MainWindow.xaml.cs (280 lines)
├── Window Management
│   ├── InitializeWindow()
│   ├── SetupMicaBackdrop()
│   └── Position Persistence
│
├── Event Handlers
│   ├── ServerAddressTextBox_TextChanged()
│   ├── ResolutionNumberBox_ValueChanged()
│   ├── MonitorComboBox_SelectionChanged()
│   ├── SetResolution_*() (3 methods)
│   └── LaunchButton_Click()
│
├── Validation
│   └── UpdateLaunchButtonState()
│
└── RDP Operations
    └── GenerateRdpContent()
```

### 3. Data Flow

```
User Input
    │
    ▼
Input Validation ──────┐
    │                  │
    ▼                  ▼
Enable/Disable   Store State
Launch Button    (X, Y, W, H)
    │                  │
    ▼                  │
Launch Button    ◄─────┘
Clicked
    │
    ▼
Generate RDP File
(Temp Location)
    │
    ▼
Execute mstsc.exe
    │
    ▼
Show Status ────┐
    │           │
    ▼           ▼
Stay Open   Clean Temp File
(2s delay)
```

## Dependency Graph

```
RdpLauncher.csproj
    │
    ├── Microsoft.Windows.SDK.BuildTools (10.0.22621.3233)
    │   └── Windows SDK APIs
    │
    ├── Microsoft.WindowsAppSDK (1.5.240802000)
    │   ├── WinUI 3 Controls
    │   ├── Windowing APIs
    │   └── Mica Backdrop
    │
    └── Microsoft.Graphics.Win2D (1.2.0)
        └── 2D Graphics Support
```

## Build Output Structure

When built on Windows, the output structure will be:

```
RdpLauncher/bin/Debug/net8.0-windows10.0.22621.0/win-x64/
├── RdpLauncher.exe                    # Main executable
├── RdpLauncher.dll                    # Application assembly
├── Microsoft.WindowsAppSDK.*.dll      # Runtime dependencies
├── Microsoft.Windows.SDK.*.dll        # SDK dependencies
├── WinRT.Runtime.dll                  # WinRT interop
└── Assets/                            # Copied assets
```

## Runtime Data Locations

### Settings File
```
%LOCALAPPDATA%\RdpLauncher\rdp-launcher-settings.txt
    WindowX=<position>
    WindowY=<position>
```

### Temporary RDP Files
```
%TEMP%\<random>.rdp
    (Created and deleted during RDP launch)
```

## Code Statistics

| Category | Files | Lines of Code | Comments |
|----------|-------|---------------|----------|
| **XAML** | 2 | ~160 | Declarative UI |
| **C#** | 2 | ~300 | Business logic |
| **PowerShell** | 1 | ~90 | Reference |
| **Documentation** | 5 | ~1000 | Markdown |
| **Config** | 4 | ~100 | Project files |
| **Total** | 14 | ~1650 | Fully implemented |

## External Dependencies

### Required at Build Time
- .NET 8.0 SDK
- Windows SDK (10.0.22621.0+)
- NuGet packages (Microsoft.WindowsAppSDK, etc.)

### Required at Runtime
- .NET 8.0 Runtime
- Windows 10 1809+ or Windows 11
- Windows App SDK Runtime
- mstsc.exe (built into Windows)

### Optional
- Visual Studio 2022 (for GUI development)
- VS Code (for text-based development)

## Size Estimates

| Component | Size (approx) |
|-----------|---------------|
| Source Code | ~50 KB |
| Documentation | ~100 KB |
| Compiled App (self-contained) | ~150 MB |
| Compiled App (framework-dependent) | ~500 KB |
| Runtime Memory | ~40 MB |

## Quick Reference

### Essential Files for Development
1. `MainWindow.xaml` - UI layout
2. `MainWindow.xaml.cs` - Logic implementation
3. `RdpLauncher.csproj` - Dependencies and configuration

### Essential Files for Understanding
1. `README.md` - Start here
2. `DEVELOPMENT.md` - Deep dive
3. `IMPLEMENTATION.md` - Design decisions

### Essential Files for Testing
1. `TESTING.md` - Test procedures
2. `reference/launch-rdp.ps1` - Expected behavior

## Version Control

```
Total commits: 3
Files tracked: 15
Git ignored: bin/, obj/, .vs/, *.user, *.rdp, etc.
Branch: copilot/add-powershell-script-for-app
```

---

**Last Updated**: 2026-02-03
**Status**: ✅ Implementation Complete (Pending Windows Testing)
