namespace ALViN.Data.Utility;

using System.Collections;
using ALViN.Data.Objects;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.VisualBasic;
using pocketbase.net;

public static class StorageManager
{
    private static readonly string logName = "StorageManager";
    private static bool isConnected = false;
    //Database Manager Singleton
    //Make Configurable
    public static Uri databaseLocation = new(Settings.GetSetting("DatabaseLocation"));
    public static Pocketbase database = new(databaseLocation.ToString());
    private static async Task Connect()
    {
        if (!isConnected){
            isConnected = true;
            database = new(databaseLocation.ToString());
            var test = database.Collections("devices").GetFullList();
            Console.WriteLine(test.Result);
            Logger.Log(logName,"Connected to database @ " + databaseLocation.ToString());
        }
    }
    private static void Disconnect()
    {

    }

    private static List<Device> decodeDevices(){

        List<Device> devices = [];
        return devices;
    }

    public static List<Device> GetDevices(){
        Task.Run(() => Connect());
        //var test = database.Collections("devices").GetFullList().Result;
        return new();
    }

    public static Hashtable LoadSettings(){
        return null;
    }

    public static void SaveSettings(){

    }

}

