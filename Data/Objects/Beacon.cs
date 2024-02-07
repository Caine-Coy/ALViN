namespace ALViN.Data.Objects;

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
