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
    //Database Manager Singleton
    //Make Configurable
    public static String databaseLocation;


    private static void Connect()
    {
        databaseLocation = new(Settings.GetSetting("DatabaseLocation"));
        using (var db = new LiteDatabase(databaseLocation)){
            var collection = db.GetCollection<Device>("Devices");
            if (collection.FindOne("Name=Test") != null){
                var device = new Device{
                Name = "Test",
                LastUuid = Guid.Empty,
                LastDetected = DateAndTime.Now,
                };
                collection.Insert(device);
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
        //var test = database.Collections("devices").GetFullList().Result;
        return new();
    }

    public static Hashtable LoadSettings(){
        return null;
    }

    public static void SaveSettings(){

    }

}

