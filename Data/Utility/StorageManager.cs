namespace ALViN.Data.Utility;

using System.Collections;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using ALViN.Data.Objects;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.VisualBasic;
public static class StorageManager
{
    private static readonly string logName = "StorageManager";
    private static Uri PocketBaseURL = new(Settings.GetSetting("DatabaseLocation"));
    private static Hashtable settings;

    //private static bool LoggedIn = false;

    private static readonly HttpClient HttpClient = new HttpClient()
    {
        BaseAddress = PocketBaseURL,
    };

    private static JsonSerializerOptions SerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new DateTimeConverter() }
        };
        return options;
    }
    private static async Task<List<Device>> JoinBeaconAndDevices(List<Device> devices)
    {
        List<Beacon> beacons = await GetBeaconsAsync();
        if (beacons != null)
        {
            foreach (Device device in devices)
            {
                foreach (Beacon beacon in beacons)
                {
                    if (device.LastBeaconID == beacon.Id)
                    {
                        device.LastBeacon = beacon;
                        break;
                    }

                }
                if (device.LastBeacon == null)
                {
                    Logger.Warn(logName, $"Not Found Beacon ID {device.LastBeaconID} Before.");
                }
            }
        }
        return devices;
    }
    public static async Task<List<Beacon>?> GetBeaconsAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync($"{PocketBaseURL}/collections/beacons/records");
            var content = await response.Content.ReadAsStringAsync();
            List<Beacon> beacons = JsonSerializer.Deserialize<BeaconResponse>(content, SerializerOptions()).Items;
            if (beacons != null)
            {
                Logger.Log(logName, $"Found {beacons.Count} beacons in database.");
                return beacons;
            }
            else
            {
                Logger.Log(logName, $"Failed to find any beacons. Response: {content}");
                return null;
            }

        }
        catch (Exception ex)
        {
            Logger.Error(logName, ex.ToString());
            return null;
        }
    }
    public static async Task<List<Device>>? GetDevicesAsync()
    {
        try
        {
            var response = await HttpClient.GetAsync($"{PocketBaseURL}/collections/devices/records");
            var content = await response.Content.ReadAsStringAsync();
            List<Device> devices = JsonSerializer.Deserialize<DeviceResponse>(content, SerializerOptions()).Items;
            if (devices != null){
                Logger.Log(logName, $"Found {devices.Count} devices in database");
                devices = await JoinBeaconAndDevices(devices);
                return devices;
            }
            else{
                Logger.Error(logName, "Empty Device List");
                return new List<Device>();
            }
            
        }
        catch (Exception ex)
        {
            Logger.Error(logName, $"Failed with exception {ex}");
            return new List<Device>();
        }


    }

    public async static Task<bool> AddBeaconToDatabase(Beacon beacon)
    {
        var response = await HttpClient.PostAsJsonAsync($"{PocketBaseURL}/collections/beacons/records", beacon);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }
        else
        {
            Logger.Error(logName, $"Failed to add beacon to database! Reason : {response.StatusCode}");
            return false;
        }
    }

    public async static Task<bool> DeleteBeaconFromDatabase(Beacon beacon)
    {
        var response = await HttpClient.DeleteAsync($"{PocketBaseURL}/collections/beacons/records/{beacon.Id}");
        Logger.Log(logName, $"Deleted Beacon {beacon.Id}, Response : {response.StatusCode}");
        return response.IsSuccessStatusCode;
    }


    public static Hashtable LoadSettings()
    {
        if (settings != null)
        {
            return settings;
        }
        else
        {
            return Settings.NewSettings();
        }
    }
    public static Hashtable DefaultSettings()
    {
        settings = Settings.NewSettings();
        return settings;
    }

    public static void SaveSettings(Hashtable _settings)
    {
        settings = _settings;
        PocketBaseURL = new(Settings.GetSetting("DatabaseLocation"));
    }

}

