namespace ALViN.Data.Utility;

using System.Collections;
using ALViN.Data.Objects;
using LiteDB;
using Microsoft.Extensions.WebEncoders.Testing;
using Microsoft.VisualBasic;
public static class StorageManager
{
    private static readonly string logName = "StorageManager";
    //Database Manager Singleton
    private static LiteDatabase database = null;
    private static List<Device> devices;
    //Make Configurable
    private static String databaseLocation = Directory.GetCurrentDirectory() + Settings.GetSetting("DatabaseLocation");

    private static void EnsureDatabaseConnection()
    {
        if (database == null)
        {
            databaseLocation = Directory.GetCurrentDirectory() + Settings.GetSetting("DatabaseLocation");
            string connectionString = $@"Filename={databaseLocation};Mode=Shared";
            database = new LiteDatabase(connectionString);
            Logger.Log(logName, $@"Database Connection Initialized. Database at {databaseLocation}.");
        }
    }

    private static void InitDatabase()
    {
        EnsureDatabaseConnection();

        if (Settings.GetBoolSetting("Debug"))
        {
            var collection = database.GetCollection<Device>("Devices");

            if (!collection.Exists(x => x.Name == "Test"))
            {
                var device = new Device
                {
                    Name = "Test",
                    LastUuid = Guid.Empty,
                    LastDetected = DateTime.Now,
                };
                collection.Insert(device);
                Logger.Log(logName, "Added new test device to base at " + databaseLocation);
            };

            var _testDevices = collection.Find(x => x.Name == "Test").ToList();
            foreach (Device device in _testDevices)

            {
                device.LastDetected = DateTime.Now;
                Logger.Log(logName, "Updated Test Device " + device.Name + " To Time " + device.LastDetected);
                collection.Update(device);
            }
        }
    }

public static List<Device> GetDevices()
{
    EnsureDatabaseConnection();
    var collection = database.GetCollection<Device>("Devices");
    devices = collection.FindAll().ToList();
    
    return devices;
}

public static Hashtable LoadSettings()
{
    return null;
}

public static void SaveSettings()
{

}

}

