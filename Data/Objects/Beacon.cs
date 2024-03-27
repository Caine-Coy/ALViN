namespace ALViN.Data.Objects;
using System.Xml;
using System.Xml.Serialization;

public class Beacon
{
    public Guid Uuid;
    public string? Name {get; set;}
    public List<Device> localDevices;

    public Beacon(){
        localDevices = new();
    }


}
