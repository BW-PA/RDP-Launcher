using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT;

namespace RdpLauncher
{
    // PInvoke for DPI functions
    internal static class PInvoke
    {
        internal static class User32
        {
            [DllImport("user32.dll")]
            internal static extern uint GetDpiForWindow(IntPtr hwnd);
        }
    }

    public sealed partial class MainWindow : Window
    {
        private AppWindow m_appWindow;
        private object m_backdropController;
        private SystemBackdropConfiguration m_configurationSource;
        private const string SettingsFileName = "rdp-launcher-settings.txt";
        
        // RDP window positioning (for the remote desktop session)
        private int rdpPositionX = 0;
        private int rdpPositionY = 25;  // Default Y offset for taskbar
        private int rdpWindowRight = 2532;  // Default to left position
        private int rdpWindowBottom = 2037;
        
        // Launcher window positioning (for this app's window)
        private int launcherWindowX = 0;
        private int launcherWindowY = 0;

        public MainWindow()
        {
            try
            {
                this.InitializeComponent();
                
                SetupMicaBackdrop();
                InitializeWindow();
                LoadWindowPosition();
                
                // Set window size after the window is activated (fully loaded)
                this.Activated += MainWindow_Activated;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MainWindow constructor: {ex}");
                throw;
            }
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            // Only run once
            this.Activated -= MainWindow_Activated;
            
            // Get DPI scaling factor
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var dpi = PInvoke.User32.GetDpiForWindow(hWnd);
            double scaleFactor = dpi / 96.0; // 96 DPI is 100% scaling
            
            // Calculate scaled window size
            // Use a smaller base height now that we removed the MinHeight constraint
            // The window will grow when InfoBar is shown
            int scaledWidth = (int)(600 * scaleFactor);
            int scaledHeight = (int)(750 * scaleFactor);
            
            // Set the window size now that it's fully loaded
            if (m_appWindow != null)
            {
                m_appWindow.Resize(new SizeInt32(scaledWidth, scaledHeight));
            }
        }

        private void InitializeWindow()
        {
            // Get the AppWindow
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            m_appWindow = AppWindow.GetFromWindowId(windowId);

            // Set title bar
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                var titleBar = m_appWindow.TitleBar;
                titleBar.ExtendsContentIntoTitleBar = true;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                
                // Set the title bar element
                SetTitleBar(AppTitleBar);
            }

            // Handle window closing to save position
            m_appWindow.Closing += OnWindowClosing;
        }

        private void SetupMicaBackdrop()
        {
            // Setup backdrop for modern Windows 11 look
            // Using DesktopAcrylicBackdrop to match Windows Explorer's subtle effect
            try
            {
                if (DesktopAcrylicController.IsSupported())
                {
                    m_configurationSource = new SystemBackdropConfiguration();
                    var acrylicController = new DesktopAcrylicController();
                    
                    // Add as backdrop target using WinRT.As extension
                    acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                    acrylicController.SetSystemBackdropConfiguration(m_configurationSource);
                    
                    m_backdropController = acrylicController;
                }
                else if (MicaController.IsSupported())
                {
                    // Fallback to Mica if Acrylic not available
                    m_configurationSource = new SystemBackdropConfiguration();
                    var micaController = new MicaController();
                    micaController.Kind = MicaKind.BaseAlt; // More subtle than Base
                    
                    micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                    micaController.SetSystemBackdropConfiguration(m_configurationSource);
                    
                    m_backdropController = micaController;
                }
            }
            catch (Exception)
            {
                // Backdrop not available, continue without it
                m_backdropController = null;
            }
        }

        private void LoadWindowPosition()
        {
            try
            {
                string settingsPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RdpLauncher",
                    SettingsFileName
                );

                if (File.Exists(settingsPath))
                {
                    string[] lines = File.ReadAllLines(settingsPath);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("WindowX="))
                        {
                            int.TryParse(line.Substring(8), out launcherWindowX);
                        }
                        else if (line.StartsWith("WindowY="))
                        {
                            int.TryParse(line.Substring(8), out launcherWindowY);
                        }
                    }

                    // Apply saved position (but not size - we want to keep our default size)
                    if (launcherWindowX != 0 || launcherWindowY != 0)
                    {
                        m_appWindow.Move(new PointInt32(launcherWindowX, launcherWindowY));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading window position: {ex.Message}");
            }
        }

        private void SaveWindowPosition()
        {
            try
            {
                string settingsDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "RdpLauncher"
                );

                Directory.CreateDirectory(settingsDir);

                string settingsPath = Path.Combine(settingsDir, SettingsFileName);
                
                var position = m_appWindow.Position;
                var settings = new List<string>
                {
                    $"WindowX={position.X}",
                    $"WindowY={position.Y}"
                };

                File.WriteAllLines(settingsPath, settings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving window position: {ex.Message}");
            }
        }

        private void OnWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            SaveWindowPosition();
        }

        private void StatusInfoBar_Closed(InfoBar sender, InfoBarClosedEventArgs args)
        {
            // Collapse the InfoBar when closed to reclaim space
            StatusInfoBar.Visibility = Visibility.Collapsed;
            
            // Shrink window back to original size
            ShrinkWindowAfterInfoBar();
        }

        private void ExpandWindowForInfoBar()
        {
            // Get DPI scaling factor
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var dpi = PInvoke.User32.GetDpiForWindow(hWnd);
            double scaleFactor = dpi / 96.0;
            
            // Expand window to accommodate InfoBar (add ~100px for InfoBar height)
            int scaledWidth = (int)(600 * scaleFactor);
            int scaledHeight = (int)(850 * scaleFactor);
            
            if (m_appWindow != null)
            {
                m_appWindow.Resize(new SizeInt32(scaledWidth, scaledHeight));
            }
        }

        private void ShrinkWindowAfterInfoBar()
        {
            // Get DPI scaling factor
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var dpi = PInvoke.User32.GetDpiForWindow(hWnd);
            double scaleFactor = dpi / 96.0;
            
            // Shrink window back to original compact size
            int scaledWidth = (int)(600 * scaleFactor);
            int scaledHeight = (int)(750 * scaleFactor);
            
            if (m_appWindow != null)
            {
                m_appWindow.Resize(new SizeInt32(scaledWidth, scaledHeight));
            }
        }

        private void ServerAddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLaunchButtonState();
        }

        private void ResolutionNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            UpdateLaunchButtonState();
        }

        private void MonitorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Parse the selected position from the ComboBox tag
            // These coordinates are designed for a 5K2K ultrawide monitor (5120×2160)
            // Tag format: "X,Y,Right,Bottom" matching the winposstr format
            // Example: "0,25,2532,2037" for left position with 2532×2012 window
            if (MonitorComboBox.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                string[] parts = tag.Split(',');
                if (parts.Length == 4)
                {
                    int.TryParse(parts[0], out rdpPositionX);
                    int.TryParse(parts[1], out rdpPositionY);
                    int.TryParse(parts[2], out rdpWindowRight);
                    int.TryParse(parts[3], out rdpWindowBottom);
                    
                    // Update the Width/Height boxes to reflect the preset dimensions
                    int width = rdpWindowRight - rdpPositionX;
                    int height = rdpWindowBottom - rdpPositionY;
                    WidthNumberBox.Value = width;
                    HeightNumberBox.Value = height;
                }
            }
        }

        private void UpdateLaunchButtonState()
        {
            if (LaunchButton == null || WidthNumberBox == null || HeightNumberBox == null || ServerAddressTextBox == null)
                return;
        
            LaunchButton.IsEnabled = !string.IsNullOrWhiteSpace(ServerAddressTextBox.Text) &&
                                     WidthNumberBox.Value >= 800 &&
                                     HeightNumberBox.Value >= 600;
        }

        private void SetResolution_2532x2012(object sender, RoutedEventArgs e)
        {
            WidthNumberBox.Value = 2532;
            HeightNumberBox.Value = 2012;
        }

        private void SetResolution_1920x1080(object sender, RoutedEventArgs e)
        {
            WidthNumberBox.Value = 1920;
            HeightNumberBox.Value = 1080;
        }

        private void SetResolution_2560x1440(object sender, RoutedEventArgs e)
        {
            WidthNumberBox.Value = 2560;
            HeightNumberBox.Value = 1440;
        }

        private void SetResolution_3840x2160(object sender, RoutedEventArgs e)
        {
            WidthNumberBox.Value = 3840;
            HeightNumberBox.Value = 2160;
        }

        private async void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serverAddress = ServerAddressTextBox.Text.Trim();

                // Create RDP file
                string rdpContent = GenerateRdpContent(serverAddress, rdpPositionX, rdpPositionY, rdpWindowRight, rdpWindowBottom);
                string tempRdpFile = Path.GetTempFileName() + ".rdp";
                File.WriteAllText(tempRdpFile, rdpContent);

                // Launch RDP
                var processInfo = new ProcessStartInfo
                {
                    FileName = "mstsc.exe",
                    Arguments = $"\"{tempRdpFile}\"",
                    UseShellExecute = true
                };

                Process.Start(processInfo);

                // Calculate dimensions for display message
                int width = rdpWindowRight - rdpPositionX;
                int height = rdpWindowBottom - rdpPositionY;

                // Show success message
                StatusInfoBar.Visibility = Visibility.Visible;
                StatusInfoBar.Severity = InfoBarSeverity.Success;
                StatusInfoBar.Title = "RDP Launched";
                StatusInfoBar.Message = $"Connection to {serverAddress} at {width}×{height}";
                StatusInfoBar.IsOpen = true;
                
                // Expand window to show InfoBar
                ExpandWindowForInfoBar();

                // Clean up temp file after a delay
                await System.Threading.Tasks.Task.Delay(2000);
                try
                {
                    if (File.Exists(tempRdpFile))
                    {
                        File.Delete(tempRdpFile);
                    }
                }
                catch { /* Ignore cleanup errors */ }

                // Keep launcher open per requirements
            }
            catch (Exception ex)
            {
                StatusInfoBar.Visibility = Visibility.Visible;
                StatusInfoBar.Severity = InfoBarSeverity.Error;
                StatusInfoBar.Title = "Launch Failed";
                StatusInfoBar.Message = ex.Message;
                StatusInfoBar.IsOpen = true;
                
                // Expand window to show InfoBar
                ExpandWindowForInfoBar();
            }
        }

        private string GenerateRdpContent(string serverAddress, int posX, int posY, int right, int bottom)
        {
            // Calculate window dimensions from coordinates
            int windowWidth = right - posX;
            int windowHeight = bottom - posY;
            
            // Use fixed desktop resolution with smart sizing enabled
            // This allows the RDP session to scale to the window size
            int desktopWidth = 1480;
            int desktopHeight = 1150;
            
            return $@"full address:s:{serverAddress}
screen mode id:i:1
use multimon:i:0
desktopwidth:i:{desktopWidth}
desktopheight:i:{desktopHeight}
smart sizing:i:1
winposstr:s:0,1,{posX},{posY},{right},{bottom}
session bpp:i:32
compression:i:1
keyboardhook:i:2
audiocapturemode:i:0
videoplaybackmode:i:1
connection type:i:7
networkautodetect:i:1
bandwidthautodetect:i:1
displayconnectionbar:i:1
enableworkspacereconnect:i:0
remoteappmousemoveinject:i:1
disable wallpaper:i:0
allow font smoothing:i:0
allow desktop composition:i:0
disable full window drag:i:1
disable menu anims:i:1
disable themes:i:0
disable cursor setting:i:0
bitmapcachepersistenable:i:1
audiomode:i:0
redirectprinters:i:1
redirectlocation:i:0
redirectcomports:i:0
redirectsmartcards:i:1
redirectwebauthn:i:1
redirectclipboard:i:1
redirectposdevices:i:0
autoreconnection enabled:i:1
authentication level:i:2
prompt for credentials:i:0
negotiate security layer:i:1
remoteapplicationmode:i:0
alternate shell:s:
shell working directory:s:
gatewayhostname:s:
gatewayusagemethod:i:4
gatewaycredentialssource:i:4
gatewayprofileusagemethod:i:0
promptcredentialonce:i:0
gatewaybrokeringtype:i:0
use redirection server name:i:0
rdgiskdcproxy:i:0
kdcproxyname:s:
enablerdsaadauth:i:0
drivestoredirect:s:C:\;
";
        }
    }
}
