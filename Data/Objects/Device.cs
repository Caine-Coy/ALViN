using System.Text.Json.Serialization;

namespace ALViN.Data.Objects;

public class Device
{  
    [JsonPropertyName("id")]
    public string Id {get;set;}
    [JsonPropertyName("name")]
    public string? Name {get; set;} 
    [JsonPropertyName("lastuuid")]
    public string LastUuid {get; set;}
    [JsonPropertyName("lastdetected")]
    public DateTime LastDetected {get; set;}
    [JsonPropertyName("lastbeacon")]
    public string LastBeaconID{get; set;}
    [JsonIgnore]
    public Beacon LastBeacon{get; set;}
    public Device(){

    }
}