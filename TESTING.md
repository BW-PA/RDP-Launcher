# Testing & Verification Guide

## Build Verification

This project requires a Windows environment to build and test, as it uses WinUI 3 which is Windows-specific.

### On Windows

1. **Prerequisites Check**
   ```powershell
   # Verify .NET SDK
   dotnet --version
   # Should be 8.0 or higher
   
   # Verify Windows SDK (in PowerShell)
   Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows Kits\Installed Roots" -Name KitsRoot10
   ```

2. **Restore and Build**
   ```powershell
   cd RDP-Launcher
   dotnet restore
   dotnet build -c Debug
   ```

3. **Expected Output**
   - Successful NuGet package restoration
   - No compilation errors
   - Executable generated in `RdpLauncher/bin/Debug/net8.0-windows10.0.22621.0/win-x64/`

### Testing Checklist

#### UI Testing
- [ ] Application launches successfully
- [ ] Window displays with Mica backdrop (on Windows 11)
- [ ] Title bar shows "RDP Launcher" with custom styling
- [ ] All UI elements render correctly:
  - [ ] Server address text box
  - [ ] Width number box (default: 1920)
  - [ ] Height number box (default: 1080)
  - [ ] Resolution preset buttons (3 buttons)
  - [ ] Monitor position combo box (5 options)
  - [ ] Launch button (initially disabled)

#### Functionality Testing
- [ ] **Input Validation**
  - [ ] Launch button disabled when server address is empty
  - [ ] Launch button enabled when valid server address entered
  - [ ] Number boxes accept valid values (Width: 800-7680, Height: 600-4320)
  
- [ ] **Resolution Presets**
  - [ ] 1920×1080 button sets correct values
  - [ ] 2560×1440 button sets correct values
  - [ ] 3840×2160 button sets correct values
  
- [ ] **Monitor Position**
  - [ ] Can select different monitor positions
  - [ ] Each position correctly sets X,Y coordinates
  
- [ ] **RDP Launch**
  - [ ] Clicking Launch button opens RDP connection
  - [ ] Success message appears in InfoBar
  - [ ] Launcher window stays open after launch
  - [ ] RDP window appears at correct screen position
  - [ ] RDP window has correct resolution
  - [ ] Temporary .rdp file is cleaned up
  
- [ ] **Window Position Persistence**
  - [ ] Move launcher window to a specific position
  - [ ] Close the application
  - [ ] Relaunch the application
  - [ ] Window appears at the same position
  - [ ] Settings file exists: `%LOCALAPPDATA%\RdpLauncher\rdp-launcher-settings.txt`

#### Edge Cases
- [ ] Very large resolutions (7680×4320) work correctly
- [ ] Minimum resolutions (800×600) work correctly
- [ ] Special characters in server address are handled
- [ ] Network unavailable scenarios (RDP should show its own error)
- [ ] Multiple rapid launches work correctly

#### Visual Quality
- [ ] Mica backdrop visible on Windows 11
- [ ] UI scales correctly at different DPI settings
- [ ] Text is readable and properly aligned
- [ ] Buttons have hover effects
- [ ] Focus indicators are visible
- [ ] Dark mode is supported (via system theme)

### Manual Test Scenarios

#### Scenario 1: Basic RDP Launch
1. Launch RDP Launcher
2. Enter server address: `localhost` (or test server)
3. Keep default 1920×1080 resolution
4. Keep Primary Monitor position
5. Click Launch
6. **Expected**: RDP connection opens at 1920×1080, launcher stays open

#### Scenario 2: Multi-Monitor Setup
1. Launch on a system with multiple monitors
2. Enter server address
3. Select "Secondary Monitor Right (1920, 0)"
4. Click Launch
5. **Expected**: RDP window appears on the right monitor

#### Scenario 3: 4K Resolution
1. Launch RDP Launcher
2. Enter server address
3. Click "3840×2160" preset button
4. Click Launch
5. **Expected**: RDP connection opens at 4K resolution

#### Scenario 4: Window Position Memory
1. Launch RDP Launcher
2. Move window to bottom-right corner of screen
3. Close application
4. Relaunch application
5. **Expected**: Window appears in bottom-right corner

### Known Limitations

1. **Linux/Mac**: Cannot build or run (Windows-only WinUI 3 app)
2. **Windows 10**: Mica backdrop will not appear (gracefully degrades to standard window)
3. **Monitor Positions**: Currently hard-coded values (per requirements)
4. **RDP Settings**: Limited to preset configurations

### Troubleshooting Build Issues

#### Issue: "Windows SDK not found"
**Solution**: Install Windows 11 SDK (10.0.22621.0 or later)
```powershell
# Download from:
# https://developer.microsoft.com/en-us/windows/downloads/windows-sdk/
```

#### Issue: "WindowsAppSDK not found"
**Solution**: Install via NuGet package manager or Visual Studio
```powershell
dotnet add package Microsoft.WindowsAppSDK --version 1.5.240802000
```

#### Issue: "Cannot restore packages"
**Solution**: Clear NuGet cache and retry
```powershell
dotnet nuget locals all --clear
dotnet restore --force
```

#### Issue: "Project doesn't load in Visual Studio"
**Solution**: Ensure Visual Studio 2022 with Windows App SDK workload is installed

### Automated Testing

Currently, this project does not include automated tests. Future improvements could include:
- Unit tests for RDP file generation logic
- UI automation tests using WinAppDriver
- Integration tests for settings persistence

### Performance Testing

Monitor these metrics:
- **Startup Time**: Should be < 2 seconds on modern hardware
- **Memory Usage**: Should be < 50 MB idle
- **RDP Launch Time**: Should be < 1 second from click to RDP window

### Security Considerations

- Temporary .rdp files are created in system temp directory
- Files are deleted after 2 second delay
- No credentials are stored or managed by the app
- Settings file only contains window position (X, Y coordinates)

## Verification Status

**Build Environment**: Linux (GitHub Actions runner)
**Build Status**: ⚠️ Cannot build Windows-specific WinUI 3 app on Linux

**Next Steps**: 
- Build and test on Windows 11 development machine
- Verify all features work as expected
- Test on different screen configurations
- Validate with real RDP servers

---

*This document should be updated after Windows-based testing is completed.*
