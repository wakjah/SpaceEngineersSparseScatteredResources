using Sandbox.Definitions;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using VRage.Game;
using VRage.Game.Components;
using VRage.Utils;

namespace SparseScatteredResourcesMod
{
    class DefinitionModifier
    {
        private delegate void Callable();

        private List<Callable> _undoCommands = new List<Callable>();

        public void Set<T, U>(T definition, Func<T, U> getter, Action<T, U> setter, U value)
        {
            U originalValue = getter(definition);
            setter(definition, value);
            _undoCommands.Add(() => setter(definition, originalValue));
        }

        public void UnsetAll()
        {
            foreach (var command in _undoCommands)
            {
                command();
            }
        }

        public int Count
        {
            get
            {
                return _undoCommands.Count;
            }
        }
    }

    public class Logger
    {
        public static void Log(string msg)
        {
            MyLog.Default.WriteLineAndConsole($"[SparseScatteredResources]: {msg}");

            //if (MyAPIGateway.Session?.Player != null)
            //    MyAPIGateway.Utilities.ShowNotification($"[ ERROR: {GetType().FullName}: {e.Message} | Send SpaceEngineers.Log to mod author ]", 10000, MyFontEnum.Red);
        }

        public static void Error(string msg)
        {
            Log("ERROR: " + msg);
        }
    }

    public class ModSettingsUtilities
    {
        public static bool SettingsFileExists<T>(string filename)
        {
            return MyAPIGateway.Utilities.FileExistsInWorldStorage(filename, typeof(T));
        }

        public static T LoadSettingsFile<T>(string filename)
        {
            try
            {
                using (var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage(filename, typeof(T)))
                {
                    string configcontents = reader.ReadToEnd();
                    T config = MyAPIGateway.Utilities.SerializeFromXML<T>(configcontents);
                    Logger.Log("Loaded existing settings from file " + filename);
                    return config;
                }
            }
            catch (Exception e)
            {
                Logger.Error("Failed to load settings from " + filename + ": " + e.Message);
                return default(T);
            }
        }

        public static T LoadOrWriteDefault<T>(T defaultSettings, string filename)
        {
            if (!SettingsFileExists<T>(filename))
            {
                Logger.Log("Configuration file not found: " + filename + ". Using default configuration instead");
                return SaveSettingsFile(defaultSettings, filename);
            }
            else
            {
                return LoadSettingsFile<T>(filename);
            }
        }

        public static T SaveSettingsFile<T>(T settings, string filename)
        {
            try
            {
                using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage(filename, typeof(T)))
                {
                    writer.Write(MyAPIGateway.Utilities.SerializeToXML<T>(settings));
                }
                Logger.Log("Wrote settings to " + filename);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to write settings to " + filename + ": " + e.Message);
            }

            return settings;
        }
    }

    public class OreConfiguration
    {
        public string OreName = "";

        public OreConfiguration()
        {}

        public OreConfiguration(string oreName)
        {
            OreName = oreName;
        }
    }

    public class ModSettings
    {
        public OreConfiguration[] AsteroidOres = new OreConfiguration[] {
            new OreConfiguration("Stone"),
            new OreConfiguration("Iron"),
            new OreConfiguration("Silver")
        };

        public static ModSettings Load()
        {
            return ModSettingsUtilities.LoadOrWriteDefault(new ModSettings(), "Settings.xml");
        }

        public static ModSettings Save(ModSettings settings)
        {
            return ModSettingsUtilities.SaveSettingsFile(settings, "Settings.xml");
        }

        public OreConfiguration GetAsteroidOreConfiguration(string ore)
        {
            return Array.Find(AsteroidOres, candidate => String.Equals(candidate.OreName, ore, StringComparison.OrdinalIgnoreCase));
        }
    }

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Session : MySessionComponentBase
    {

        private DefinitionModifier _modifier = new DefinitionModifier();
        private ModSettings _settings;

        public override void LoadData()
        {
            _settings = ModSettings.Load();
            
            Logger.Log("Setting up asteroid ore spawning");
            MyDefinitionManager definitions = MyDefinitionManager.Static;
            foreach (var definition in definitions.GetVoxelMaterialDefinitions())
            {
                if (!definition.SpawnsInAsteroids)
                {
                    // Don't mess with it if it isn't already an asteroid spawning item
                    continue;
                }

                var oreConfiguration = _settings.GetAsteroidOreConfiguration(definition.MinedOre);
                bool enabled = oreConfiguration != null;
                
                Logger.Log("Setting asteroid ore " + definition.MinedOre + " enabled=" + enabled);
                _modifier.Set(definition, d => d.SpawnsInAsteroids, (d, v) => d.SpawnsInAsteroids = v, enabled);

                // todo separate settings for meteorites
                _modifier.Set(definition, d => d.SpawnsFromMeteorites, (d, v) => d.SpawnsFromMeteorites = v, enabled);
            }

            Logger.Log("Made " + _modifier.Count + " modifications");
        }

        protected override void UnloadData()
        {
            _modifier.UnsetAll();
            Logger.Log("Undid " + _modifier.Count + " modifications");
        }
    }
}
