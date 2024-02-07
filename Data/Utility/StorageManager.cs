namespace ALViN.Data.Utility;

using System.Collections;
using ALViN.Data.Objects;

public static class StorageManager
{
    private static readonly string logName = "StorageManager";
    //Database Manager Singleton

    public static Uri databaseLocation;
    private static void Connect()
    {

    }
    private static void Disconnect()
    {

    }

    public static List<Device> GetDevices(){
        return new List<Device>();
    }

    public static Hashtable LoadSettings(){
        return null;
    }

    public static void SaveSettings(){

    }

}

