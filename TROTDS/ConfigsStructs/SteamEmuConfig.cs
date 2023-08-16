using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS.ConfigsStructs
{
    public class SteamEmuConfig
    {
        [JsonProperty("steamemu_lang")]
        public SteamEmuLang Lang { get; set;}

        [JsonProperty("steamemu_nick")]
        public string Nick { get; set; }

        public SteamEmuConfig()
        {
            Lang = SteamEmuLang.english;
            Nick = "gabe";
        }
    }
}
