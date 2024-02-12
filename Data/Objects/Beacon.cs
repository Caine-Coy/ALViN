namespace ALViN.Data.Objects;
using System.Xml;
using System.Xml.Serialization;

[XmlRootAttribute("PurchaseOrder",
IsNullable = false)]
public class Beacon
{
    private Guid uuid;
    private ushort major;
    private ushort minor;
    private string name {get;}

    public List<Device> localDevices;

    Beacon(Guid uuid,string name){
        this.uuid = uuid;

        this.name = name;
        localDevices = new();
    }


}
