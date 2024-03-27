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
            string connectionString = $@"Filename={databaseLocation};Connection=Shared";
            database = new LiteDatabase(connectionString);
            Logger.Log(logName, $@"Database Connection Initialized. Database at {databaseLocation}.");
            InitDatabase();
        }
    }

    private static void InitDatabase()
    {
        EnsureDatabaseConnection();
         var beaconCollection = database.GetCollection<Beacon>("Beacons");
         var deviceCollection = database.GetCollection<Device>("Devices");
        if (Settings.GetBoolSetting("Debug"))
        {
            if (!beaconCollection.Exists(x => x.Name == "Test")){
                var beacon = new Beacon
                {
                    Name = "Test",
                    Uuid = Guid.Empty,
                };
                beaconCollection.Insert(beacon);
                Logger.Log(logName,$@"Added new test beacon to beacon collection at {databaseLocation}");
            if (!deviceCollection.Exists(x => x.Name == "Test"))
            {
                var device = new Device
                {
                    Name = "Test",
                    LastUuid = Guid.Empty,
                    LastDetected = DateTime.Now,
                    LastBeacon = beacon,
                };
                deviceCollection.Insert(device);
                Logger.Log(logName, $@"Added new test device to device collection at {databaseLocation}");
            };
            }

            var _testDevices = deviceCollection.Find(x => x.Name == "Test").ToList();
            foreach (Device device in _testDevices)

            {
                device.LastDetected = DateTime.Now;
                Logger.Log(logName, "Updated Test Device " + device.Name + " To Time " + device.LastDetected);
                deviceCollection.Update(device);
            }
        }
    }

public static List<Device> GetDevices()
{
    EnsureDatabaseConnection();
    var deviceCollection = database.GetCollection<Device>("Devices");
    devices = deviceCollection.FindAll().ToList();
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

