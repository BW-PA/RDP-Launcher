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
    public sealed partial class MainWindow : Window
    {
        private AppWindow m_appWindow;
        private MicaController m_backdropController;
        private SystemBackdropConfiguration m_configurationSource;
        private const string SettingsFileName = "rdp-launcher-settings.txt";
        private int positionX = 0;
        private int positionY = 0;

        public MainWindow()
        {
            try
            {
                this.InitializeComponent();
                
                // Simplified initialization - skip custom features for now
                Title = "RDP Launcher";
                
                // Try to get window handle
                IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                m_appWindow = AppWindow.GetFromWindowId(windowId);
                
                if (m_appWindow != null)
                {
                    m_appWindow.Resize(new SizeInt32(600, 700));
                }
                
                // Skip LoadWindowPosition and SetupMicaBackdrop for now
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in MainWindow constructor: {ex}");
                throw;
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

            // Set window size
            m_appWindow.Resize(new SizeInt32(600, 700));

            // Handle window closing to save position
            m_appWindow.Closing += OnWindowClosing;
        }

        private void SetupMicaBackdrop()
        {
            // Setup Mica backdrop for modern Windows 11 look
            try
            {
                if (MicaController.IsSupported())
                {
                    m_configurationSource = new SystemBackdropConfiguration();
                    m_backdropController = new MicaController();
                    m_backdropController.Kind = MicaKind.Base;
                    
                    // Add as backdrop target using WinRT.As extension
                    m_backdropController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
                    m_backdropController.SetSystemBackdropConfiguration(m_configurationSource);
                }
            }
            catch (Exception)
            {
                // Mica backdrop not available, continue without it
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
                            int.TryParse(line.Substring(8), out int x);
                            positionX = x;
                        }
                        else if (line.StartsWith("WindowY="))
                        {
                            int.TryParse(line.Substring(8), out int y);
                            positionY = y;
                        }
                    }

                    // Apply saved position
                    if (positionX != 0 || positionY != 0)
                    {
                        m_appWindow.Move(new PointInt32(positionX, positionY));
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
            if (MonitorComboBox.SelectedItem is ComboBoxItem item && item.Tag is string tag)
            {
                string[] parts = tag.Split(',');
                if (parts.Length == 2)
                {
                    int.TryParse(parts[0], out positionX);
                    int.TryParse(parts[1], out positionY);
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
                int width = (int)WidthNumberBox.Value;
                int height = (int)HeightNumberBox.Value;

                // Create RDP file
                string rdpContent = GenerateRdpContent(serverAddress, width, height, positionX, positionY);
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

                // Show success message
                StatusInfoBar.Severity = InfoBarSeverity.Success;
                StatusInfoBar.Title = "RDP Launched";
                StatusInfoBar.Message = $"Connection to {serverAddress} at {width}×{height}";
                StatusInfoBar.IsOpen = true;

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
                StatusInfoBar.Severity = InfoBarSeverity.Error;
                StatusInfoBar.Title = "Launch Failed";
                StatusInfoBar.Message = ex.Message;
                StatusInfoBar.IsOpen = true;
            }
        }

        private string GenerateRdpContent(string serverAddress, int width, int height, int posX, int posY)
        {
            return $@"screen mode id:i:2
use multimon:i:0
desktopwidth:i:{width}
desktopheight:i:{height}
session bpp:i:32
winposstr:s:0,{posX},{posY},{posX + width},{posY + height}
compression:i:1
keyboardhook:i:2
audiocapturemode:i:0
videoplaybackmode:i:1
connection type:i:7
networkautodetect:i:1
bandwidthautodetect:i:1
displayconnectionbar:i:1
enableworkspacereconnect:i:0
disable wallpaper:i:0
allow font smoothing:i:0
allow desktop composition:i:0
disable full window drag:i:1
disable menu anims:i:1
disable themes:i:0
disable cursor setting:i:0
bitmapcachepersistenable:i:1
full address:s:{serverAddress}
audiomode:i:0
redirectprinters:i:1
redirectcomports:i:0
redirectsmartcards:i:1
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
";
        }
    }
}
