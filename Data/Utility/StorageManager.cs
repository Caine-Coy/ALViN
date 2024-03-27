namespace ALViN.Data.Utility;

using System.Collections;
using ALViN.Data.Objects;
using LiteDB;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.VisualBasic;

public static class StorageManager
{
    private static readonly string logName = "StorageManager";
    private static bool isConnected = false;
    private static LiteDatabase database;
    //Database Manager Singleton
    //Make Configurable
    public static String databaseLocation;


    private static void Connect()
    {
        databaseLocation = new(Directory.GetCurrentDirectory()+Settings.GetSetting("DatabaseLocation"));
        database = new LiteDatabase(databaseLocation);
        var collection = database.GetCollection<Device>("Devices");
        using (database){
            if (!collection.Exists(x => x.Name == "Test")){
                var device = new Device{
                Name = "Test",
                LastUuid = Guid.Empty,
                LastDetected = DateAndTime.Now,
                };
                collection.Insert(device);
                Logger.Log(logName,("Added new test device to base at " + databaseLocation));
            };
        } 
        if (!isConnected){
            isConnected = true;
            Logger.Log(logName,"Connected to database @ " + databaseLocation.ToString());
        }
    }
    private static void Disconnect()
    {

    }       
    public static List<Device> GetDevices(){
        Connect();
        Logger.Log(logName,"Connected to database @ " + databaseLocation.ToString());
        return new();
    }

    public static Hashtable LoadSettings(){
        return null;
    }

    public static void SaveSettings(){

    }

}

