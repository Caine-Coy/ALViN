using ALViN.Data.Objects;
using System.Xml;
using System.Xml.Serialization;

public static class SerializationController{

    public static void SerializeBeacon(Beacon beacon){
        XmlSerializer serializer =
        new XmlSerializer(typeof(Beacon));
        TextWriter writer = new StreamWriter("test.xml");
        serializer.Serialize(writer,beacon);

    }

    public static void SerializeDevice(){

    }
}