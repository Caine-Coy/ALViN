namespace ALViN.Data.Utility;

using System.Collections;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ALViN.Data.Objects;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.VisualBasic;
public static class StorageManager
{
    private static readonly string logName = "StorageManager";
    private static readonly Uri PocketBaseURL = new(Settings.GetSetting("DatabaseLocation"));


    private static bool LoggedIn = false;

    private static readonly HttpClient HttpClient = new HttpClient()
    {
        BaseAddress = PocketBaseURL,
    };

    private class LoginResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }

    private static JsonSerializerOptions SerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new DateTimeConverter() }
        };
        return options;
    }

    public static async Task<string?> LoginAndGetTokenAsync(string email, string password)
    {
        var loginURL = $"{PocketBaseURL}/collections/devices/auth-with-password";

        var loginData = new { email, password };
        Logger.Log(logName, $"{loginURL} with {loginData}");
        var response = await HttpClient.PostAsJsonAsync(loginURL, loginData);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<LoginResponse>(content, SerializerOptions());
            LoggedIn = true;
            return tokenResponse?.Token;
        }
        else
        {
            LoggedIn = false;
            Logger.Error(logName, $"Failed to Log into database with message {response.Content.ReadAsStringAsync().Result}");
            return null;
        }
    }

    public static void SetAuthorizationHeader(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
    private static async Task<List<Device>> JoinBeaconAndDevices(List<Device> devices)
    {
        var response = await HttpClient.GetAsync($"{PocketBaseURL}/collections/beacons/records");
        var content = await response.Content.ReadAsStringAsync();

        List<Beacon> beacons = JsonSerializer.Deserialize<BeaconResponse>(content, SerializerOptions()).Items;
        Logger.Log(logName, $"Found {beacons.Count} beacons in database.");
        if (beacons != null)
        {
            foreach (Device device in devices)
            {
                Logger.Log(logName, $"Trying to match Device BeaconID {device.LastBeaconID} with known Beacons");
                foreach (Beacon beacon in beacons)
                {
                    if (device.LastBeaconID == beacon.Uuid) device.LastBeacon = beacon;
                    break;
                }
                if (device.LastBeacon != null)
                {
                    Logger.Log(logName, $"Last seen {device.Name} near {device.LastBeacon.Name} at {device.LastDetected}");
                }
                else{
                    Logger.Warn(logName,$"Not Found Beacon ID {device.LastBeaconID} Before.");
                }
            }
        }
        return devices;
    }

    public static async Task<List<Device>>? GetDevicesAsync()
    {
        if (!LoggedIn)
        {
            var token = await LoginAndGetTokenAsync(Settings.GetSetting("DatabaseEmail"), Settings.GetSetting("DatabasePassword"));
            SetAuthorizationHeader(token);
        }

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


    public static Hashtable LoadSettings()
    {
        return null;
    }

    public static void SaveSettings()
    {

    }

}

