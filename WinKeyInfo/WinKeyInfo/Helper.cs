using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Management;
using System.Threading;
using System.Diagnostics;

namespace WinKeyInfo
{
    internal class Helper
    {
        public static MainWindow mainWindow = new MainWindow();
        public static LType CL = LType.CN;//current language string
        public static void MainWindowLoad(MainWindow MThis)
        {
            mainWindow = MThis;

            mainWindow.Loaded += (object sender, RoutedEventArgs e) =>
            {

            };
            mainWindow.Closed += (object sender, EventArgs e) =>
            {
                Process.GetCurrentProcess().Kill();
            };
            mainWindow.CN.Click += LClick;
            mainWindow.EN.Click += LClick;
            mainWindow.About.Click += Aboutlick;
            Task.Run(() =>
            {
                //Control initialization
                mainWindow.Dispatcher.Invoke(() =>
                {
                    mainWindow.CPUValue.Text = "";
                    mainWindow.GPUValue.Text = "";
                    mainWindow.RAMValue.Text = "";
                    mainWindow.DiskDriveValue.Text = "";
                    mainWindow.BaseBoardValue.Text = "";
                    mainWindow.SoundDeviceValue.Text = "";
                    mainWindow.DisplayValue.Text = "";
                    mainWindow.BatteryValue.Text = "";
                });
                //Get CPU information Win32_processor
                ManagementClass CPUmanagementClass = new ManagementClass("Win32_processor");
                ManagementObjectCollection CPUmanagementBaseObjects = CPUmanagementClass.GetInstances();
                string CPUInfo = "";
                foreach (ManagementObject Management in CPUmanagementBaseObjects)
                {
                    string CPUName = "", NumberOfCores = "", ThreadCount = "", L2Cache = "", L3Cache = "";
                    CPUName = Management["Name"].ToString().Trim();
                    NumberOfCores = Management["NumberOfCores"].ToString().Trim();
                    ThreadCount = Management["ThreadCount"].ToString().Trim();
                    //L2 Cache size (unit: KB) and convert to MB
                    L2Cache = Management["L2CacheSize"] != null ? (Convert.ToInt32(Management["L2CacheSize"]) / 1024).ToString().Trim() + " MB" : "0 MB";
                    //L3 Cache size (unit: KB) and convert to MB
                    L3Cache = Management["L3CacheSize"] != null ? (Convert.ToInt32(Management["L3CacheSize"]) / 1024).ToString().Trim() + " MB" : "0 MB";
                    CPUInfo += CPUName + "   " + "Cores:" + NumberOfCores + "   " + "Threads:" + ThreadCount + "   " + "L2Cache:" + L2Cache + "   " + "L3Cache:" + L3Cache + "\n";
                }
                CPUInfo = CPUInfo.Remove(CPUInfo.Length - 1);
                mainWindow.CPUValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.CPUValue.Text = CPUInfo;
                });
                //Get CPU information
                //Get graphics card information Win32_DisplayConfiguration
                ManagementClass GPUmanagementClass = new ManagementClass("Win32_DisplayConfiguration");
                ManagementObjectCollection GPUmanagementBaseObjects = GPUmanagementClass.GetInstances();
                string GPUInfo = "";
                foreach (ManagementObject Management in GPUmanagementBaseObjects)
                {
                    GPUInfo += Management["Caption"].ToString().Trim() + "\n";
                }
                GPUInfo = GPUInfo.Remove(GPUInfo.Length - 1);
                mainWindow.GPUValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.GPUValue.Text = GPUInfo;
                });
                //Get graphics card information
                //Get memory stick information Win32_PhysicalMemory
                string RAMInfo = "", RAMSpeed = "", RAMManufacturer = "", RAMPartNumber = "", RAMDeviceLocator = "", SMBIOSMemoryType = "", MemoryType = "";
                int RAMGB = 0, TRAMGB = 0;
                ManagementClass RAMmanagementClass = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection RAMmanagementBaseObjects = RAMmanagementClass.GetInstances();
                foreach (ManagementObject Management in RAMmanagementBaseObjects)
                {
                    RAMSpeed = Management["Speed"].ToString().Trim();
                    TRAMGB = Convert.ToInt32(Convert.ToInt64(Management["Capacity"].ToString().Trim()) / 1024 / 1024 / 1024);
                    RAMGB += TRAMGB;
                    RAMManufacturer = Management["Manufacturer"].ToString().Trim();
                    RAMPartNumber = Management["PartNumber"].ToString().Trim();
                    RAMDeviceLocator = Management["DeviceLocator"].ToString().Trim();
                    SMBIOSMemoryType = Management["SMBIOSMemoryType"].ToString().Trim();
                    MemoryType = GetMemoryType(SMBIOSMemoryType);
                    if (RAMSpeed != "" && TRAMGB != 0 && RAMManufacturer != "" && RAMPartNumber != "" && RAMDeviceLocator != "")
                    {
                        RAMInfo += RAMManufacturer + " " + " " + RAMPartNumber + " " + MemoryType + " " + RAMSpeed + " " + TRAMGB + "GB" + " (" + RAMDeviceLocator + ")\n";
                        RAMSpeed = ""; RAMManufacturer = ""; RAMPartNumber = ""; RAMDeviceLocator = "";
                        TRAMGB = 0;
                    }
                }
                RAMInfo = RAMInfo.Remove(RAMInfo.Length - 1);
                mainWindow.RAMValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.RAMValue.Text = RAMInfo + "\n=   " + RAMGB + "GB";
                });
                //Get memory stick information
                //get hard drive Win32_DiskDrive
                ManagementClass DiskDrivemanagementClass = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection DiskDrivemanagementBaseObjects = DiskDrivemanagementClass.GetInstances();
                string DiskDriveInfo = "";
                foreach (ManagementObject Management in DiskDrivemanagementBaseObjects)
                {
                    DiskDriveInfo += Management["Caption"].ToString().Trim() + "\n";
                }
                DiskDriveInfo = DiskDriveInfo.Remove(DiskDriveInfo.Length - 1);
                mainWindow.DiskDriveValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.DiskDriveValue.Text = DiskDriveInfo;
                });
                //get hard drive
                //Get motherboard information Win32_BaseBoard
                string BaseBoardManufacturer = "", BaseBoardProduct = "";
                ManagementClass BaseBoardmanagementClass = new ManagementClass("Win32_BaseBoard");
                ManagementObjectCollection BaseBoardmanagementBaseObjects = BaseBoardmanagementClass.GetInstances();
                string BaseBoardInfo = "";
                foreach (ManagementObject Management in BaseBoardmanagementBaseObjects)
                {
                    BaseBoardManufacturer = Management["Manufacturer"].ToString().Trim();
                    BaseBoardProduct = Management["Product"].ToString().Trim();
                    if (BaseBoardManufacturer != "" && BaseBoardProduct != "")
                    {
                        BaseBoardInfo += BaseBoardManufacturer + "   " + BaseBoardProduct + "\n";
                    }
                }
                BaseBoardInfo = BaseBoardInfo.Remove(BaseBoardInfo.Length - 1);
                mainWindow.BaseBoardValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.BaseBoardValue.Text = BaseBoardInfo;
                });
                //Get motherboard information
                //get a sound card Win32_SoundDevice
                ManagementClass SoundDevicemanagementClass = new ManagementClass("Win32_SoundDevice");
                ManagementObjectCollection SoundDevicemanagementBaseObjects = SoundDevicemanagementClass.GetInstances();
                string SoundDeviceInfo = "";
                foreach (ManagementObject Management in SoundDevicemanagementBaseObjects)
                {
                    SoundDeviceInfo += Management["Caption"].ToString().Trim() + "\n";
                }
                SoundDeviceInfo = SoundDeviceInfo.Remove(SoundDeviceInfo.Length - 1);
                mainWindow.SoundDeviceValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.SoundDeviceValue.Text = SoundDeviceInfo;
                });
                //get a sound card
                //get monitor Win32_DesktopMonitor
                ManagementClass DesktopMonitormanagementClass = new ManagementClass("Win32_DesktopMonitor");
                ManagementObjectCollection DesktopMonitormanagementBaseObjects = DesktopMonitormanagementClass.GetInstances();
                string DesktopMonitorInfo = "";
                foreach (ManagementObject Management in DesktopMonitormanagementBaseObjects)
                {
                    DesktopMonitorInfo += (Management["PNPDeviceID"].ToString().Trim()).Split('\\')[1].Trim() + "\n";
                }
                int horizontalResolution = 0;
                int verticalResolution = 0;
                int refreshRate = 0;
                // Gets display resolution and refresh rate
                ManagementClass videoControllerClass = new ManagementClass("Win32_VideoController");
                foreach (ManagementObject videoController in videoControllerClass.GetInstances())
                {
                    horizontalResolution = Convert.ToInt32(videoController["CurrentHorizontalResolution"]);
                    verticalResolution = Convert.ToInt32(videoController["CurrentVerticalResolution"]);
                    refreshRate = Convert.ToInt32(videoController["CurrentRefreshRate"]);
                }
                DesktopMonitorInfo = DesktopMonitorInfo.Remove(DesktopMonitorInfo.Length - 1);
                mainWindow.DisplayValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.DisplayValue.Text = DesktopMonitorInfo + $"   Resolution:{horizontalResolution}*{verticalResolution}   Refresh rate:{refreshRate}Hz";
                });
                //get monitor
                //get battery Win32_Battery
                ManagementClass BatterymanagementClass = new ManagementClass("Win32_Battery");
                ManagementObjectCollection BatterymanagementBaseObjects = BatterymanagementClass.GetInstances();
                string BatteryCaption = "", BatteryDeviceID = "", BatteryEstimatedChargeRemaining = "", BatteryInfo = "";
                foreach (ManagementObject Management in BatterymanagementBaseObjects)
                {
                    BatteryCaption = Management["Caption"].ToString().Trim();
                    BatteryDeviceID = Management["DeviceID"].ToString().Trim();
                    BatteryEstimatedChargeRemaining = Management["EstimatedChargeRemaining"].ToString().Trim();
                    BatteryInfo += BatteryCaption + "   " + BatteryDeviceID + "   (" + BatteryEstimatedChargeRemaining + "%)\n";
                }
                if (BatteryDeviceID.Trim() == "")
                {
                    BatteryInfo = "Not Found\n";
                }
                else
                {
                    BatteryInfo = BatteryInfo.Remove(BatteryInfo.Length - 1);
                }
                mainWindow.BatteryValue.Dispatcher.Invoke(() =>
                {
                    mainWindow.BatteryValue.Text = BatteryInfo;
                });
                //get battery
            });
        }
        public static void LClick(object sender, RoutedEventArgs e)
        {
            string CmenuItem = ((MenuItem)sender).Name;
            LType TL = CmenuItem == "CN" ? LType.CN : LType.EN;
            if (CL != TL)
            {
                if (TL == LType.CN)
                {
                    CL = LType.CN;
                    mainWindow.CPUTitle.Content = "处理器:";
                    mainWindow.GPUTitle.Content = "显卡:";
                    mainWindow.RAMTitle.Content = "内存条:";
                    mainWindow.DiskDriveTitle.Content = "硬盘:";
                    mainWindow.BaseBoardTitle.Content = "主板:";
                    mainWindow.SoundDeviceTitle.Content = "声卡:";
                    mainWindow.DisplayTitle.Content = "显示器:";
                    mainWindow.BatteryTitle.Content = "电池:";
                }
                else
                {
                    CL = LType.EN;
                    mainWindow.CPUTitle.Content = "CPU:";
                    mainWindow.GPUTitle.Content = "GPU:";
                    mainWindow.RAMTitle.Content = "RAM:";
                    mainWindow.DiskDriveTitle.Content = "DiskDrive:";
                    mainWindow.BaseBoardTitle.Content = "BaseBoard:";
                    mainWindow.SoundDeviceTitle.Content = "SoundDevice:";
                    mainWindow.DisplayTitle.Content = "Display:";
                    mainWindow.BatteryTitle.Content = "Battery:";
                }
            }
        }
        public static void Aboutlick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/haotian888/WinKeyInfo");
        }
        public static string GetMemoryType(string SMBIOSMemoryType)
        {
            string MemoryType = "";
            switch (SMBIOSMemoryType)
            {
                case "20":
                    MemoryType = "DDR";
                    break;
                case "21":
                    MemoryType = "DDR2";
                    break;
                case "24":
                    MemoryType = "DDR3";
                    break;
                case "26":
                    MemoryType = "DDR4";
                    break;
                case "27":
                    MemoryType = "DDR5";
                    break;
                case "_":
                    MemoryType = "Unknown";
                    break;
                default:
                    break;
            }
            return MemoryType;
        }
    }
    public enum LType
    {
        CN, EN
    }
}
