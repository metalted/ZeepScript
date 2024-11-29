using System;
using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using System.IO;
using ZeepSDK.ChatCommands;
using BepInEx;

namespace ZeepScript
{
    public interface ILuaFunction
    {
        string Namespace { get; }
        string Name { get; }
        Delegate CreateFunction();
    }

    public interface ILuaEvent
    {
        string Name { get; }
        void Subscribe();
        void Unsubscribe();
    }

    public class ListenToLuaFunction : ILuaFunction
    {
        public string Namespace => "ZeepScript";
        public string Name => "ListenTo";

        public Delegate CreateFunction()
        {
            // Explicitly create a delegate for the Implementation method
            return new Action<string>(Implementation);
        }

        private void Implementation(string eventNameArg)
        {
            LuaManager.ListenTo(eventNameArg);
        }
    }

    public static class LuaManager
    {
        private static bool init;
        private static Script lua;
        private static bool loaded;
        private static List<ILuaEvent> RegisteredEvents = new List<ILuaEvent>();
        private static List<ILuaEvent> ListenedToEvents = new List<ILuaEvent>();

        public static Action OnRegister;
        public static Action OnLoaded;
        public static Action OnUnloaded;       

        public static void Initialize()
        {
            if (init)
            {
                return;
            }

            RegisterChatCommands();            
            init = true;
        }

        public static void LoadScriptFile(string path)
        {
            if (loaded)
            {
                UnloadScript();
            }

            if (LoadScript(path))
            {
                ScriptLoaded();
            }
            else
            {
                Unsubscribe();
            }
        }        
        public static void UnloadScript()
        {
            OnUnloaded?.Invoke();
            CallFunction("OnUnload");
            Unsubscribe();
            loaded = false;
        }
        public static void ListenTo(string eventName)
        {
            ILuaEvent luaEvent = RegisteredEvents.FirstOrDefault(e => e.Name == eventName);
            if (luaEvent == null)
            {
                Plugin.Instance.Log($"Event '{eventName}' not found.");
                return;
            }

            if (ListenedToEvents.Any(e => e.Name == luaEvent.Name))
            {
                Plugin.Instance.Log($"Event '{eventName}' is already being listened to.");
                return;
            }

            try
            {
                luaEvent.Subscribe();
                ListenedToEvents.Add(luaEvent);
                Plugin.Instance.Log($"Started listening to event '{eventName}'.");
            }
            catch (Exception ex)
            {
                Plugin.Instance.Log($"Error subscribing to event '{eventName}': {ex.Message}");
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

        public static void RegisterFunction<TFunction>()
        where TFunction : ILuaFunction, new()
        {
            RegisterFunction(typeof(TFunction));
        }
        public static void RegisterType<T>()
        {
            if (UserData.IsTypeRegistered(typeof(T)))
            {
                Plugin.Instance.Log($"Type '{typeof(T).FullName}' is already registered. Skipping.");
                return;
            }

            try
            {
                UserData.RegisterType<T>();
                Plugin.Instance.Log($"Successfully registered type '{typeof(T).FullName}'.");
            }
            catch (Exception ex)
            {
                Plugin.Instance.Log($"Error registering type '{typeof(T).FullName}': {ex.Message}");
            }
        }
        public static void RegisterEvent<TEvent>() where TEvent : ILuaEvent, new()
        {
            var luaEvent = new TEvent();
            if (RegisteredEvents.Any(e => e.Name == luaEvent.Name)) return;
            RegisteredEvents.Add(luaEvent);
        }

        private static void ScriptLoaded()
        {
            OnLoaded?.Invoke();
            CallFunction("OnLoad");
            loaded = true;
        }
        private static bool LoadScript(string path)
        {
            lua = new Script(CoreModules.None);

            RegisterAllFunctionsInCurrentAssembly();
            RegisterAllEventsInCurrentAssembly();

            OnRegister?.Invoke();

            if (File.Exists(path))
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
        
        private static void RegisterAllFunctionsInCurrentAssembly()
        {
            var types = typeof(LuaManager).Assembly.GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass)
                .Where(x => typeof(ILuaFunction).IsAssignableFrom(x))
                .ToList();

            foreach (var type in types)
            {
                RegisterFunction(type);
            }
        }

        private static void RegisterFunction(Type functionType)
        {
            if (functionType == null)
                throw new ArgumentNullException(nameof(functionType));

            if (!typeof(ILuaFunction).IsAssignableFrom(functionType))
                throw new ArgumentException(nameof(functionType));

            var function = Activator.CreateInstance(functionType) as ILuaFunction;

            var namespaceTable = lua.Globals.Get(function.Namespace).Table;
            if (namespaceTable == null)
            {
                namespaceTable = new Table(lua);
                lua.Globals[function.Namespace] = namespaceTable;
            }

            var existingFunction = namespaceTable.Get(function.Name);
            if (existingFunction == null)
            {
                namespaceTable[function.Name] = function.CreateFunction();
                Plugin.Instance.Log($"Registered: {function.Namespace}.{function.Name}");
            }
            else
            {
                Plugin.Instance.Log($"Skipped: {function.Namespace}.{function.Name} (already exists)");
            }
        }
        
        private static void RegisterAllEventsInCurrentAssembly()
        {
            var eventTypes = typeof(LuaManager).Assembly.GetTypes()
                .Where(t => typeof(ILuaEvent).IsAssignableFrom(t) && !t.IsAbstract);

            foreach (var type in eventTypes)
            {
                var luaEvent = Activator.CreateInstance(type) as ILuaEvent;
                if (luaEvent != null && RegisteredEvents.All(e => e.Name != luaEvent.Name))
                {
                    RegisteredEvents.Add(luaEvent);
                }
            }
        }

        private static void Unsubscribe()
        {
            foreach (var luaEvent in ListenedToEvents)
            {
                try
                {
                    luaEvent.Unsubscribe();
                }
                catch (Exception ex)
                {
                    Plugin.Instance.Log($"Error unsubscribing from event '{luaEvent.Name}': {ex.Message}");
                }
            }

            ListenedToEvents.Clear();
        }
    
        private static void TryLoadLuaFromPluginsFolder(string name)
        {
            string searchPattern = $"{name}.lua";

            // Search for the file in the directory
            string[] luaFiles = Directory.GetFiles(Paths.PluginPath, searchPattern, SearchOption.AllDirectories);

            if (luaFiles.Length > 0)
            {
                LoadScriptFile(luaFiles[0]);
            }
        }

        private static void RegisterChatCommands()
        {
            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zeepscript load",
                "Loads the scripts from the plugins folder by name.",
                arguments => {

                    if (!string.IsNullOrWhiteSpace(arguments))
                    {
                        TryLoadLuaFromPluginsFolder(arguments.Trim());
                    }
                }
            );

            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zeepscript unload",
                "Unloads the current script if there is any, and unsubscribes from any events.",
                arguments => {
                    UnloadScript();
                }
            );

            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zs load",
                "Loads the scripts from the plugins folder by name.",
                arguments => {

                    if (!string.IsNullOrWhiteSpace(arguments))
                    {
                        TryLoadLuaFromPluginsFolder(arguments.Trim());
                    }
                }
            );

            ChatCommandApi.RegisterLocalChatCommand(
                "/",
                "zs unload",
                "Unloads the current script if there is any, and unsubscribes from any events.",
                arguments => {
                    UnloadScript();
                }
            );
        }
    }
}
