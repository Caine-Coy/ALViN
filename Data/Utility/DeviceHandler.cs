namespace ALViN.Data.Utility;

using System.Diagnostics;
using System.Reflection.Metadata;
using ALViN.Data.Objects;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

public static class DeviceHandler
{
    private static readonly string logName = "DeviceHandler";
    private static TimeSpan timeSinceSeenCutoff;
    private static List<Device> devices = new();
    private static List<Device> recentDevices = new();

    public static void CheckSettings()
    {
        var timeCutoff = Settings.GetIntSetting("TimeSinceSeenCutoff");
        //Makes new Timespan of 0, hours, minutes from the settings, and 0 seconds.
        timeSinceSeenCutoff = new(0, timeCutoff, 0);
    }

    public static async Task<List<Device>>? GetCurrentDevicesAsync()
    {
        recentDevices.Clear();
        devices = await GetAllDevicesAsync();
        DateTime now = DateTime.UtcNow;
        int timeCutoff = Settings.GetIntSetting("TimeSinceSeenCutoff");

        foreach (Device device in devices)
        {
            Logger.Log(logName, $"Device {device.Name} Last seen at {device.LastDetected}. Threshold is {timeCutoff} Minutes, this was seen {(int)now.Subtract(device.LastDetected).TotalMinutes} Minutes ago.");
            if (now.Subtract(device.LastDetected).TotalMinutes < timeCutoff)
            {
                recentDevices.Add(device);
            }
        }
        Logger.Log(logName, $@"Found {recentDevices.Count} tracked within the last {timeCutoff} Minutes.");
        return recentDevices;
    }

    public static async Task<List<Device>> GetAllDevicesAsync()
    {
        devices = await StorageManager.GetDevicesAsync();
        if (devices == null)
        {
            devices = new();
            Logger.Error(logName, "Device List is Null.");
        }
        return devices;
    }

    public static async Task<bool> DeleteBeacon(Beacon beacon){
        bool result = await StorageManager.DeleteBeaconFromDatabase(beacon);
        if (result) Logger.Log(logName,$"Successfully Deleted {beacon.Name}");
        return result;
    
    }
}