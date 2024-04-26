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
    private static readonly Uri PocketBaseURL = new(Settings.GetSetting("DatabaseLocation"));
    private static Hashtable settings;

    private static bool LoggedIn = false;

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
                Logger.Log(logName, $"Trying to match Device BeaconID {device.LastBeacon} with known Beacons");
                foreach (Beacon beacon in beacons)
                {
                    if (device.LastBeaconID == beacon.Id)
                    {
                        device.LastBeacon = beacon;
                        break;
                    }

                }
                if (device.LastBeacon != null)
                {
                    Logger.Log(logName, $"Last seen {device.Name} near {device.LastBeacon.Name} at {device.LastDetected}");
                }
                else
                {
                    Logger.Warn(logName, $"Not Found Beacon ID {device.LastBeaconID} Before.");
                }
            }
        }
        return devices;
    }
    public static async Task<List<Beacon>> GetBeaconsAsync(){

        var response = await HttpClient.GetAsync($"{PocketBaseURL}/collections/beacons/records");
        var content = await response.Content.ReadAsStringAsync();
        List<Beacon> beacons = JsonSerializer.Deserialize<BeaconResponse>(content, SerializerOptions()).Items;
        Logger.Log(logName, $"Found {beacons.Count} beacons in database.");
        return beacons;
    }
    public static async Task<List<Device>>? GetDevicesAsync()
    {
        var response = await HttpClient.GetAsync($"{PocketBaseURL}/collections/devices/records");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                List<Device> devices = JsonSerializer.Deserialize<DeviceResponse>(content, SerializerOptions()).Items;
                Logger.Log(logName, $"Found {devices.Count} devices in database");
                devices = await JoinBeaconAndDevices(devices);
                return devices;
            }
            catch (JsonException e)
            {
                Logger.Error(logName, $"{e.ToString()} with message {content}");
            }
        }
        else
        {
            Logger.Error(logName, await response.Content.ReadAsStringAsync());
            Logger.Error(logName, "Failed to retrieve devices.");
        }
        return new List<Device>();

    }

    public async static Task<bool> AddBeaconToDatabase(Beacon beacon){
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

    public async static Task<bool> DeleteBeaconFromDatabase(Beacon beacon){
        var response = await HttpClient.DeleteAsync($"{PocketBaseURL}/collections/beacons/{beacon.Id}");
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
    }

}

