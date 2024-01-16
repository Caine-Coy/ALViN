namespace ALViN.Data.Utility;

using System.Collections;
using System.Diagnostics;
using ALViN.Data.Objects;
using Microsoft.AspNetCore.Server.HttpSys;

public static class Logger
{
    private static readonly string logName = "Logger";
    private static int verbosity;
    private static Stack<String> log = new();

    public static void Startup(){
        verbosity = Settings.GetIntSetting("Verbosity");
        Log(logName,"Logger starting.");
    }

    public static void Log(String callingObject,string msg){
        composeMessage(callingObject,msg,2);
    }

       public static void Warn(String callingObject,string msg){
        composeMessage(callingObject,"WARNING!: " + msg,1);
    }

    public static void Error(String callingObject,string msg){
        composeMessage(callingObject,"ERROR!: " + msg,0);
    }

    private static void composeMessage(String callingObject,string msg, int msgVerbosity){
        string message = DateTime.Now.ToString() + " : " +callingObject.ToString() + ":" + msg;
        if (msgVerbosity <= verbosity){
            log.Push(message);
        }
    }


}
