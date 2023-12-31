﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TROTDS.ConfigsStructs;
using TROTDS.Properties;

namespace TROTDS
{
    public class SettingsService
    {
        public CombineConfig Config { get; set; }
        public void Start()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "/config.json";
            try
            {
                Config = JsonConvert.DeserializeObject<CombineConfig>(File.ReadAllText(path));
                
            }
            catch
            {
            }

            if (Config is null)
            {
                Config = new CombineConfig();
            }

            Save();
        }

        public void Save()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "/config.json";
            try
            {
                var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch
            {
            }
        }
    }
}
