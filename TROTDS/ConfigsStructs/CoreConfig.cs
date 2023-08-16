using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS.ConfigsStructs
{
    public class CoreConfig
    {
        [JsonProperty("app_path")]
        public string AppPath { get; set; }

        public CoreConfig()
        {
            AppPath = "";
        }
    }
}
