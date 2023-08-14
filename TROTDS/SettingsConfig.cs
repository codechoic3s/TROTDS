using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS
{
    public class SettingsConfig
    {
        [JsonProperty("game_path")]
        public string GamePath { get; set; }

        [JsonProperty("game_lang")]
        public SteamEmuLang GameLang { get; set;}

        [JsonProperty("game_nick")]
        public string GameNick { get; set; }

        public SettingsConfig()
        {
            GamePath = "";
            GameLang = SteamEmuLang.english;
            GameNick = "parasha";
        }
    }
}
