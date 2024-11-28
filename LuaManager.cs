using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using System.IO;

namespace ZeepScript
{
    public static class LuaManager
    {
        private static Script lua;
        private static bool loaded;

        public static void Initialize()
        {
            UserData.RegisterType<ZeepkistClient.ZeepkistNetworkPlayer>();
            UserData.RegisterType<LevelScriptableObject>();
        }

        public static void LoadScript(string path)
        {
            if(loaded)
            {
                UnloadScript();
            }

            if(LoadScriptFile(path))
            {
                CallFunction("OnLoad");
                loaded = true;
            }
            else
            {
                loaded = false;
            }
        }

        public static void UnloadScript()
        {
            CallFunction("OnUnload");
            Events.Unsubscribe();
            loaded = false;
        }

        private static bool LoadScriptFile(string path)
        {
            lua = new Script(CoreModules.None);
            RegisterFunctions();

            if(File.Exists(path))
            {
                string content = File.ReadAllText(path);
                try
                {
                    lua.DoString(content);
                    return true;
                }
                catch (InterpreterException ex)
                {
                    Plugin.Instance.Log($"Lua Error: {ex.DecoratedMessage}");
                    return false;
                }
            }
            else
            {
                Plugin.Instance.Log($"Lua file not found: {path}");
                return false;
            }
        }

        private static void RegisterFunctions()
        {
            RegisterNamespace("Leaderboard", LuaFunctions.Leaderboard);
            RegisterNamespace("Lobby", LuaFunctions.Lobby);
            RegisterNamespace("Messaging", LuaFunctions.Messaging);
            RegisterNamespace("ZeepScript", LuaFunctions.ZeepScript);
        }

        private static void RegisterNamespace(string name, Dictionary<string, Delegate> functions)
        {
            Table namespaceTable = lua.Globals.Get(name).Table;
            if (namespaceTable == null)
            {
                namespaceTable = new Table(lua);
                lua.Globals[name] = namespaceTable;
            }

            foreach (var entry in functions)
            {
                namespaceTable[entry.Key] = entry.Value;
            }
        }

        public static void CallFunction(string functionName, params object[] args)
        {
            try
            {
                var function = lua.Globals.Get(functionName);
                if (function.Type != DataType.Function)
                {
                    Plugin.Instance.Log($"Lua function '{functionName}' is not implemented. Skipping.");
                    return;
                }

                DynValue[] dynArgs = args.Select(arg => DynValue.FromObject(lua, arg)).ToArray();
                lua.Call(function, dynArgs);
            }
            catch (Exception e)
            {
                Plugin.Instance.Log($"Error calling Lua function '{functionName}': {e.Message}");
            }
        }
    }
}
