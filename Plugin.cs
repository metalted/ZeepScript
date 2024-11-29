using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;

namespace ZeepScript
{
    [BepInPlugin(pluginGUID, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGUID = "com.metalted.zeepkist.zeepscript";
        public const string pluginName = "ZeepScript";
        public const string pluginVersion = "1.0";
        public static Plugin Instance;

        public ConfigEntry<string> scriptPath;        

        private void Awake()
        {   
            Harmony harmony = new Harmony(pluginGUID);
            harmony.PatchAll();

            Instance = this;
            LuaManager.Initialize();

            // Plugin startup logic
            Logger.LogInfo($"Plugin {pluginName} is loaded!");
        }

        public void Log(object data, bool force = false)
        {
            Logger.LogInfo(data);
        }
    }
}
