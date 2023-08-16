using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS
{
    public class Pathes
    {
        public string GamePath { get; set; }

        public string GameBinariesPath { get; set; }
        public string GameExecutablePath { get; set; }
        public string CoalescedGamePath { get; set; }
        public string PatchedGameExecutablePath { get; set; }
        public string SteamAPIPath { get; set; }
        public string SteamEmuSettingsPath { get; set; }

        public string NearPath { get; set; }

        public string TempCompressedTools { get; set; }
        public string TempTools { get; set; }

        public bool HasPatchedExecutablePath { get; set; }

        public bool VerifiedGamePath { get;  set; }

        public Pathes()
        {
            NearPath = AppDomain.CurrentDomain.BaseDirectory;
            var temppath = Path.GetTempPath();
            TempCompressedTools = temppath + "\\tempcompressedtools";
            TempTools = temppath + "\\temptools";
        }
    }
}
