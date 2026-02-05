# Quick Start Guide for Windows Testing

This guide will help you quickly test the RDP Launcher on a Windows machine.

## Prerequisites Installation

1. **Install .NET 8.0 SDK**
   ```powershell
   # Download from:
   https://dotnet.microsoft.com/download/dotnet/8.0
   
   # Verify installation:
   dotnet --version
   # Should show 8.0.x
   ```

2. **Install Visual Studio 2022** (Recommended)
   - Download Community Edition (free): https://visualstudio.microsoft.com/
   - During installation, select:
     - ✅ .NET Desktop Development
     - ✅ Windows application development
   - Or use VS Code with C# Dev Kit extension

## Quick Build & Test (5 minutes)

### Option A: Using Visual Studio 2022

1. **Clone and Open**
   ```powershell
   git clone https://github.com/BW-PA/RDP-Launcher.git
   cd RDP-Launcher
   # Double-click RdpLauncher.sln
   ```

2. **Build**
   - Press `Ctrl+Shift+B` to build
   - Or click Build → Build Solution

3. **Run**
   - Press `F5` to run with debugging
   - Or `Ctrl+F5` to run without debugging

### Option B: Using Command Line

```powershell
# Clone repository
git clone https://github.com/BW-PA/RDP-Launcher.git
cd RDP-Launcher

# Restore packages
dotnet restore

# Build
dotnet build -c Debug

# Run
dotnet run --project RdpLauncher/RdpLauncher.csproj
```

### Option C: Using VS Code

```powershell
# Clone repository
git clone https://github.com/BW-PA/RDP-Launcher.git
cd RDP-Launcher
code .

# In VS Code terminal:
dotnet restore
dotnet build
dotnet run --project RdpLauncher/RdpLauncher.csproj
```

## Expected First Run

When you first run the app, you should see:

```
✅ A window opens (600×700 pixels)
✅ Semi-transparent Mica backdrop (on Windows 11)
✅ Title bar says "RDP Launcher"
✅ Server address text box (empty)
✅ Width: 1920, Height: 1080 (default)
✅ Three resolution preset buttons
✅ Monitor position dropdown
✅ Launch button (disabled until you enter a server)
```

## 60-Second Test

1. **Launch the app**
   ```powershell
   dotnet run --project RdpLauncher/RdpLauncher.csproj
   ```

2. **Enter a server address**
   - Type: `localhost` (or any server name)
   - Notice: Launch button becomes enabled

3. **Try a resolution preset**
   - Click "2560×1440" button
   - Verify: Width and Height update

4. **Launch RDP**
   - Click "Launch RDP Connection" button
   - Expected: RDP window opens
   - Expected: Success message appears
   - Expected: Launcher stays open

5. **Move and restart**
   - Drag window to a different position
   - Close the app
   - Run again
   - Expected: Window appears at same position

## Common Issues

### Issue 1: "Windows SDK not found"
```powershell
# Install Windows 11 SDK
# https://developer.microsoft.com/windows/downloads/windows-sdk/
```

### Issue 2: "Package not found: Microsoft.WindowsAppSDK"
```powershell
# Clear NuGet cache
dotnet nuget locals all --clear
dotnet restore --force
```

### Issue 3: Build succeeds but no window appears
```powershell
# Check output directory
dir RdpLauncher\bin\Debug\net8.0-windows10.0.22621.0\win-x64\

# Try running directly
.\RdpLauncher\bin\Debug\net8.0-windows10.0.22621.0\win-x64\RdpLauncher.exe
```

### Issue 4: Mica backdrop doesn't appear
- This is normal on Windows 10
- Mica requires Windows 11
- App will work fine with standard background

## Build Configurations

### Debug Build (Recommended for testing)
```powershell
dotnet build -c Debug
```
- Includes debugging symbols
- Faster build time
- Larger file size

### Release Build (For deployment)
```powershell
dotnet build -c Release
```
- Optimized code
- Smaller file size
- No debugging symbols

## Platform Options

Build for specific platform:
```powershell
# x64 (most common)
dotnet build -c Release -r win-x64

# x86 (32-bit)
dotnet build -c Release -r win-x86

# ARM64 (Surface devices, etc.)
dotnet build -c Release -r win-arm64
```

## Self-Contained vs Framework-Dependent

### Framework-Dependent (Default, ~500 KB)
```powershell
dotnet publish -c Release
# Requires .NET 8.0 Runtime on target machine
```

### Self-Contained (~150 MB, includes runtime)
```powershell
dotnet publish -c Release -r win-x64 --self-contained true
# Can run on any Windows machine without .NET installed
```

## Quick Functional Test

Test all features in 2 minutes:

```
[ ] App launches
[ ] Mica backdrop visible (Windows 11 only)
[ ] Enter server: "test.server.com"
[ ] Launch button enables
[ ] Click "2560×1440" - values update
[ ] Click "3840×2160" - values update
[ ] Change width to 2000 manually
[ ] Change height to 1200 manually
[ ] Select different monitor position
[ ] Click Launch button
[ ] RDP window attempts to open
[ ] Success message appears
[ ] Launcher stays open
[ ] Close launcher
[ ] Relaunch
[ ] Window position preserved
```

## Where to Find Built App

After successful build:
```
RdpLauncher\
└── bin\
    └── Debug\
        └── net8.0-windows10.0.22621.0\
            └── win-x64\
                └── RdpLauncher.exe  ← Run this
```

## Performance Expectations

| Metric | Expected Value |
|--------|---------------|
| Startup time | < 2 seconds |
| Memory usage (idle) | < 50 MB |
| Window size | 600×700 pixels |
| RDP launch time | < 1 second |

## Screenshots to Take

For verification, take screenshots of:

1. **Main window** - Default state
2. **Main window with Mica** - Close-up showing backdrop
3. **After entering server** - Enabled launch button
4. **After clicking launch** - Success message
5. **RDP window opened** - Showing it worked
6. **Settings file** - In `%LOCALAPPDATA%\RdpLauncher\`

## Reporting Test Results

If you encounter issues, include:

1. **Environment Info**
   ```powershell
   # Run these commands:
   winver  # Shows Windows version
   dotnet --version  # Shows .NET version
   dotnet --list-sdks  # Shows installed SDKs
   ```

2. **Build Output**
   - Copy the complete build output
   - Include any warnings or errors

3. **Runtime Behavior**
   - What happens when you run it?
   - Any error messages?
   - Screenshots if possible

## Integration with Development

After testing, you can:

1. **Make changes** - Edit `.xaml` or `.cs` files in VS Code or Visual Studio
2. **Hot reload** - Some changes apply without restart (in Visual Studio)
3. **Debug** - Set breakpoints and step through code
4. **Package** - Create MSIX installer for distribution

## Success Criteria

✅ App builds without errors
✅ App runs on Windows 11
✅ Mica backdrop appears (Windows 11)
✅ All UI controls work
✅ RDP launches successfully
✅ Window position persists
✅ No crashes or exceptions

## Getting Help

- See **TESTING.md** for detailed test procedures
- See **DEVELOPMENT.md** for architecture details
- See **IMPLEMENTATION.md** for technical decisions
- See **PROJECT-STRUCTURE.md** for code organization

## Estimated Time

| Task | Time |
|------|------|
| Prerequisites installation | 10-20 minutes |
| Clone and first build | 3-5 minutes |
| Basic functional test | 2 minutes |
| Complete test suite | 15-30 minutes |
| **Total** | **30-60 minutes** |

---

**Ready to test?** Start with Option A, B, or C above! 🚀
