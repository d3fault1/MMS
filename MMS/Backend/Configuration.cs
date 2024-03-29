﻿using System;
using System.IO;
using System.Linq;
using MMS.DataModels;

namespace MMS.Backend
{
    static class Configuration
    {
        private static string configFile = "config.ini";
        public static ConfigurationModel Parse()
        {
            if (!File.Exists(configFile)) if (!MakeDefault()) return null;
            ConfigurationModel model = new ConfigurationModel();
            var props = typeof(ConfigurationModel).GetProperties();
            using (var reader = new StreamReader(File.OpenRead(configFile)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == String.Empty) continue;
                    if (line[0] == '#') continue;
                    var keyvaluepair = line.Split('=');
                    if (keyvaluepair.Length != 2)
                    {
                        Logging.Error("Configuration file is invalid");
                        return null;
                    }
                    var key = keyvaluepair[0].Trim();
                    var value = keyvaluepair[1].Trim();
                    var prop = props.FirstOrDefault(a => a.Name == key);
                    if (prop == null)
                    {
                        Logging.Error("Configuration file is invalid");
                        return null;
                    }
                    try
                    {
                        if (prop.PropertyType == typeof(string)) value = value.Trim('\"');
                        prop.SetValue(model, Convert.ChangeType(value, prop.PropertyType));
                    }
                    catch (Exception e)
                    {
                        Logging.Error("Configuration file is invalid. " + e.Message);
                    }
                }
            }
            return model;
        }
        public static bool MakeDefault()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(new FileStream(configFile, FileMode.OpenOrCreate)))
                {
                    writer.WriteLine("#This is the default configuration generated by the program");
                    writer.WriteLine("HTTPServerPort = 2628");
                    writer.WriteLine("DBInstance = \"SQLEXPRESS\"");
                    writer.WriteLine("DBName = \"museum_database\"");
                }
                return true;
            }
            catch(Exception e)
            {
                Logging.Error($"Could not make default configuration. {e.Message}");
                return false;
            }
        }
    }
}
