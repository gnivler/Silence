using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony;
using Newtonsoft.Json;

namespace Silence
{
    public class Core
    {
        public static string modDirectory;
        public static Settings modSettings = new Settings();

        public static void Init(string directory, string settingsJSON)
        {
            //HarmonyInstance.DEBUG = true;
            HarmonyInstance harmony = HarmonyInstance.Create("ca.gnivler.Silence");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            try
            {
                modSettings = JsonConvert.DeserializeObject<Settings>(settingsJSON);
                modDirectory = directory;
                FileLog.logPath = Path.Combine(directory, "log.txt");
                if (modSettings.EnableDebug)
                {
                    Logger.Clear();
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }

            // trying to patch the generic for voiceover  .. for reasons
            // public static uint PostEvent<T>(T eventEnumValue, AkGameObj sourceObject,
            // AkCallbackManager.EventCallback callback = null, object in_pCookie = null) where T : struct, IConvertible
            // overloaded

            //MethodInfo mi = typeof(WwiseManager).GetMethods().First(x => x.Name == "PostEvent" &&  x.IsGenericMethod);
            //MethodInfo constructed = mi.MakeGenericMethod(new Type[] {typeof(AudioEventList_vo)});
            //// constructed: UInt32 PostEvent[Enum](System.Enum, AkGameObj, AkCallbackManager+EventCallback, System.Object)
            //Logger.LogLine($"constructed: {constructed}");
            //var prefix = typeof(Patches).GetMethod("PostEventPrefix");
            //harmony.Patch(constructed, new HarmonyMethod(prefix));
        }
    }
}