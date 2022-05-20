using System;
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
                        Logging.Error(Errors.InvalidConfig);
                        return null;
                    }
                    var key = keyvaluepair[0].Trim();
                    var value = keyvaluepair[1].Trim();
                    var prop = props.FirstOrDefault(a => a.Name == key);
                    if (prop == null)
                    {
                        Logging.Error(Errors.InvalidConfig);
                        return null;
                    }
                    try
                    {
                        prop.SetValue(model, value);
                    }
                    catch (Exception e)
                    {
                        Logging.Error(Errors.InvalidConfig + ". " + e.Message);
                    }
                }
            }
            return model;
        }
    }
}
