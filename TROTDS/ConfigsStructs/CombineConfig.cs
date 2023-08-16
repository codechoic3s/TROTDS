using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TROTDS.ConfigsStructs;

namespace TROTDS.ConfigsStructs
{
    public class CombineConfig
    {
        [JsonProperty("core")]
        public CoreConfig Core { get; set; }

        [JsonProperty("steam_emu")]
        public SteamEmuConfig SteamEmu { get; set; }

        public CombineConfig()
        {
            Core = new CoreConfig();
            SteamEmu = new SteamEmuConfig();
        }
    }
}
