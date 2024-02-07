namespace ALViN.Data.Utility;

using System.Diagnostics;
using System.Reflection.Metadata;
using ALViN.Data.Objects;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

public static class DeviceHandler
{
    private static readonly string logName = "DeviceHandler";
    private static TimeSpan timeSinceSeenCutoff;
    private static List<Device> devices = LoadDevices();
    private static List<Device> recentDevices = CompileRecentDevices();

    public static void checkSettings(){
        var timeCutoff = Settings.GetIntSetting("TimeSinceSeenCutoff");
        //Makes new Timespan of 0, hours, minutes from the settings, and 0 seconds.
        timeSinceSeenCutoff = new(0,timeCutoff,0);
    }

    private static List<Device> LoadDevices()
      {
        Logger.Log(logName,"Loading device list from storage.");
        return StorageManager.GetDevices();
     }

    private static List<Device> CompileRecentDevices()
      {
        recentDevices = new List<Device>();
        DateTime now = DateTime.Now;
        int timeCutoffInt = Settings.GetIntSetting("TimeSinceSeenCutoff");
        DateTime timeCutoff = now.Subtract(TimeSpan.FromMinutes(timeCutoffInt));
        foreach (Device device in devices){
            if (device.LastDetected > now.Subtract(timeSinceSeenCutoff)){
                recentDevices.Add(device);
            }
        }
        Logger.Log(logName,"Found " + recentDevices.Count + " tracked within the last " + timeCutoffInt + " Minutes.");
        return recentDevices;
    }

    public static int getTotalDeviceAmount(){
        devices = LoadDevices();
        return devices.Count;
    } 

    public static int getCurrentDevices(){
        recentDevices = CompileRecentDevices();
        return recentDevices.Count;
    }


}