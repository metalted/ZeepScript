using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using ZeepSDK.ChatCommands;
using System.IO;

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

            scriptPath = Config.Bind("Settings", "Script Path", "", "The full path to the script.");

            LuaManager.Initialize();

            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zeepscript load",
                "Loads the scripts from the path given in the settings.",
                arguments => {

                    if (!string.IsNullOrWhiteSpace(arguments))
                    {
                        string trimmedArg = arguments.Trim();
                        string searchPattern = $"{trimmedArg}.lua";

                        // Search for the file in the directory
                        string[] luaFiles = Directory.GetFiles(Paths.PluginPath, searchPattern, SearchOption.AllDirectories);

                        if (luaFiles.Length > 0)
                        {
                            // Load the first matching Lua script
                            LuaManager.LoadScript(luaFiles[0]);
                        }
                    }
                    else
                    {
                        // Load the default script if no argument is provided
                        LuaManager.LoadScript(scriptPath.Value);
                    }
                }
            );

            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zeepscript unload",
                "Unloads the current script if there is any, and unsubscribes from any events.",
                arguments => {
                    LuaManager.UnloadScript();
                }
            );

            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zs load",
                "Loads the scripts from the path given in the settings.",
                arguments => {

                    if (!string.IsNullOrWhiteSpace(arguments))
                    {
                        string trimmedArg = arguments.Trim();
                        string searchPattern = $"{trimmedArg}.lua";

                        // Search for the file in the directory
                        string[] luaFiles = Directory.GetFiles(Paths.PluginPath, searchPattern, SearchOption.AllDirectories);

                        if (luaFiles.Length > 0)
                        {
                            // Load the first matching Lua script
                            LuaManager.LoadScript(luaFiles[0]);
                        }
                    }
                    else
                    {
                        // Load the default script if no argument is provided
                        LuaManager.LoadScript(scriptPath.Value);
                    }
                }
            );

            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zs unload",
                "Unloads the current script if there is any, and unsubscribes from any events.",
                arguments => {
                    LuaManager.UnloadScript();
                }
            );

            // Plugin startup logic
            Logger.LogInfo($"Plugin {pluginName} is loaded!");
        }

        public void Log(object data, bool force = false)
        {
            Logger.LogInfo(data);
        }
    }
}
