namespace ALViN.Data.Objects;

using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

public class Beacon
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("uuid")]
    public string Uuid {get;set;}
    [JsonPropertyName("name")]
    public string? Name {get; set;}
    [JsonIgnore]
    public List<Device> localDevices;

    public Beacon(){
        localDevices = new();
    }


}
