using System.Text.Json.Serialization;

namespace ALViN.Data.Objects;

public class Device
{  
    [JsonPropertyName("id")]
    public string Id {get;set;}
    [JsonPropertyName("Name")]
    public string? Name {get; set;} 
    [JsonPropertyName("LastUuid")]
    public string LastUuid {get; set;}
    [JsonPropertyName("LastDetected")]
    public DateTime LastDetected {get; set;}
    [JsonPropertyName("LastBeacon")]
    public string LastBeaconID{get; set;}
    [JsonIgnore]
    public Beacon LastBeacon{get; set;}
    public Device(){

    }
}