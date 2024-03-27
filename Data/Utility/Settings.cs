using System.Collections;
using System.ComponentModel;
using System.Reflection;
using ALViN.Data.Utility;
using Microsoft.VisualBasic;

namespace ALViN.Data;

public static class Settings
{
    readonly static bool debug = true;
    private static readonly string logName = "Settings";
    private static Hashtable List = LoadSettings();
    
    static int TimeSinceLastSeen;

    //This makes default settings.
    static Hashtable NewSettings(){
        Hashtable settings = new(){
            //Generic
            //0-2 Verbosity. 2: Logs, 1: Warns, 0 Errors.
            {"LoggerVerbosity",2},
            {"Debug",true},

            //Storage
            {"DatabaseLocation",@"\ALViNDatabase.db"},
            

            //Scanning
            //detection Time in minutes past you consider a device inactive
            {"TimeSinceSeenCutoff",30},
        };
        return settings;
    }

    static Hashtable LoadSettings(){
        //Try Load from storage
        List = StorageManager.LoadSettings();
        //if failed, make new default settings.
        if (List == null){
            List = NewSettings();
            Logger.Warn(logName,"Failed to load settings. Creating settings from defaults.");
        }   

        return List;
    }
    public static int GetIntSetting(string setting){
        if (List.ContainsKey(setting)){
            var result = List[setting];
            if (result != null && result.GetType() == typeof(int)){
                return (int)result;
            }
            else{
                Logger.Error(logName,"Incorrect Setting Value!");
            }
        }
        else{
            Logger.Error(logName,"Setting " + setting + " Not Found!");
        }
        return 0;
    }
    public static string GetSetting(string setting){
        if (List.ContainsKey(setting)){
            var result = List[setting];
            if (result != null){
                return (string)result;
            }
            else{
                Logger.Error(logName,"Incorrect Setting Value!");
            }
        }
        else{
            Logger.Error(logName,"Setting " + setting + " Not Found!");
        }
        return "";
    }
        public static bool GetBoolSetting(string setting){
        if (List.ContainsKey(setting)){
            bool? result = (bool)List[setting];
            if (result == null){
                Logger.Error(logName,"Setting "+ setting + " Malformed! Expected Bool Got "+ List[setting]+ ". Returning False.");
                return false;
            }
            return (bool)result;
        }
        else{
            Logger.Error(logName,"Setting " + setting + " Not Found!");
            return false;
        }
       
    }

    static void SaveSettings(){

    }

}