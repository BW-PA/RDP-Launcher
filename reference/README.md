# Reference Implementation

This folder contains the original PowerShell script that serves as the reference implementation for the RDP Launcher functionality.

## launch-rdp.ps1

The PowerShell script demonstrates the core functionality:
- Launches RDP connections with custom resolution settings
- Positions RDP windows at specific screen coordinates
- Works around scaling issues with HiDPI (4k+) monitors
- Creates temporary RDP configuration files with custom settings

### Usage Example

```powershell
.\launch-rdp.ps1 -ServerAddress "server.domain.com" -Width 1920 -Height 1080 -PositionX 0 -PositionY 0
```

This reference implementation will be replaced by a modern WinUI 3 application with a sleek user interface.
