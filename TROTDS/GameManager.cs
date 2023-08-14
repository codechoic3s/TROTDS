using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using TROTDS.LoggingPrograms;

namespace TROTDS
{
    public class GameManager
    {
        public string GamePath;

        public string GameBinariesPath;
        public string GameExecutablePath;
        public string CoalescedGamePath;
        public string patchedGameExecutablePath;
        public string SteamAPIPath;
        public string SteamAPIBakPath;
        public string SteamEmuSettingsPath;

        public string NearPath;

        public string TempCompressedTools;
        public string TempTools;

        public bool HasSteamAPIBak;
        public bool HasPatchedExecutablePath;

        public bool VerifiedGamePath;
        public bool ExtractedTools;

        public GameManager()
        {
            NearPath = AppDomain.CurrentDomain.BaseDirectory;
            TempCompressedTools = "\\tempcompressedtools";
            TempTools = "\\temptools";
        }

        public (bool, string) FullPatch(string gamePath)
        {
            var state = ValidatePath(gamePath);
            if (!state.Item1)
                return state;

            state = ExtractTools();
            if (!state.Item1)
                return state;

            state = BackupRealSteamAPI();
            if (!state.Item1)
                return state;

            state = PatchGameExecutable();
            if (!state.Item1)
                return state;

            state = LoadCustomSteamAPI();
            if (!state.Item1)
                return state;

            RemoveTemp();

            state = BackupAndReplaceCoalesced();
            if (!state.Item1)
                return state;

            return (true, "");
        }

        public (bool, string) ValidatePath(string gamePath)
        {
            GamePath = gamePath;

            if (!Directory.Exists(GamePath))
            {
                return (false, "Not real Game Path");
            }

            GameBinariesPath = GamePath + "\\Binaries";

            if (!Directory.Exists(GameBinariesPath))
            {
                return (false, "Not real /Binaries in Game Path");
            }

            CoalescedGamePath = GamePath + "\\TransGame\\Config\\PC\\Cooked\\Coalesced.ini";

            if (!File.Exists(CoalescedGamePath))
            {
                return (false, "Not real /TransGame/Config/PC/Cooked/Coalesced.ini in Game Path");
            }

            SteamEmuSettingsPath = GameBinariesPath + "\\settings";

            GameExecutablePath = GamePath + "\\Binaries\\TransGame.exe";

            if (!File.Exists(GameExecutablePath))
            {
                return (false, "Not real Game Executable in /Binaries");
            }

            patchedGameExecutablePath = GameBinariesPath + "\\TransGame.exe.unpacked.exe";
            HasPatchedExecutablePath = File.Exists(patchedGameExecutablePath);

            SteamAPIPath = GamePath + "\\Binaries\\steam_api.dll";

            if (!File.Exists(SteamAPIPath))
            {
                return (false, "Not real SteamAPI in /Binaries");
            }

            CheckSteamAPIBak();

            VerifiedGamePath = true;

            return (true, "");
        }

        public (bool, string) TryRunPatched()
        {
            if (!File.Exists(patchedGameExecutablePath))
            {
                return (false, "Not real Patched Game Executable in /Binaries as " + patchedGameExecutablePath);
            }

            try
            {
                Process.Start(patchedGameExecutablePath);
            }
            catch (Exception e)
            {
                return (false, "Failed start Patched Game Executable in /Binaries as " + patchedGameExecutablePath + " (" + e.Message + ")");
            }

            return (true, "");
        }

        public (bool, string) PatchGameExecutable()
        {
            var temppath = Path.GetTempPath();

            var steamstub_path = temppath + TempTools + "\\steamstub_patcher\\Steamless.CLI.exe";

            var plc = new ProcessLogCatcher(NearPath + "\\steamstub.txt");

            if (!File.Exists(GameExecutablePath))
            {
                return (false, "Not real Game Executable in /Binaries");
            }

            var args = $@"--realign {'"'}{GameExecutablePath}{'"'}";
            var sstate = plc.RunProgram(steamstub_path, args);

            plc.ProgramProcess.WaitForExit();

            var rstate = plc.Save();

            HasPatchedExecutablePath = File.Exists(patchedGameExecutablePath);

            return rstate;
        }

        public (bool, string) ChangeGameLang(SteamEmuLang lang)
        {
            if (!Directory.Exists(SteamEmuSettingsPath))
            {
                Directory.CreateDirectory(SteamEmuSettingsPath);
            }

            try
            {
                var writer = File.CreateText(SteamEmuSettingsPath + "\\language.txt");
                writer.Write(lang.ToString());
                writer.Close();
            }
            catch (Exception e)
            {
                return (false, "Failed to write lang (" + e.Message + ")");
            }

            return (true, "");
        }

        public (bool, string) BackupAndReplaceCoalesced()
        {
            if (!File.Exists(CoalescedGamePath))
            {
                return (false, "Not real /TransGame/Config/PC/Cooked/Coalesced.ini in Game Path");
            }

            if (!File.Exists(CoalescedGamePath + ".bak"))
            {
                try
                {
                    File.Copy(CoalescedGamePath, CoalescedGamePath + ".bak");
                }
                catch (Exception e)
                {
                    return (false, "Failed to backup Coalesced (" + e.Message + ")");
                }
            }

            try
            {
                File.WriteAllBytes(CoalescedGamePath, Properties.Resources.coalesced);
            } 
            catch (Exception e)
            {
                return (false, "Failed to write Coalesced (" + e.Message + ")");
            }

            return (true, "");
        }

        public (bool, string) ChangeGameNick(string nick)
        {
            if (!Directory.Exists(SteamEmuSettingsPath))
            {
                Directory.CreateDirectory(SteamEmuSettingsPath);
            }

            try
            {
                var writer = File.CreateText(SteamEmuSettingsPath + "\\account_name.txt");
                writer.Write(nick);
                writer.Close();
            }
            catch (Exception e)
            {
                return (false, "Failed to write nick (" + e.Message + ")");
            }

            return (true, "");
        }

        public (bool, string) LoadCustomSteamAPI()
        {
            var temppath = Path.GetTempPath();
            var steamemu_path = temppath + TempTools + "\\steamemu";
            try
            {
                File.Delete(GameBinariesPath + "\\steam_api.dll");
                File.Copy(steamemu_path + "\\steam_api.dll", GameBinariesPath + "\\steam_api.dll");

                File.Delete(GameBinariesPath + "\\local_save.txt");
                File.Copy(steamemu_path + "\\local_save.txt", GameBinariesPath + "\\local_save.txt");

                File.Delete(GameBinariesPath + "\\steam_appid.txt");
                File.Copy(steamemu_path + "\\steam_appid.txt", GameBinariesPath + "\\steam_appid.txt");

                File.Delete(GameBinariesPath + "\\steam_interfaces.txt");
                File.Copy(steamemu_path + "\\steam_interfaces.txt", GameBinariesPath + "\\steam_interfaces.txt");
            }
            catch (Exception e)
            {
                return (false, "Failed copy steamemu files (" + e.Message + ")");
            }

            return (true, "");
        }

        public (bool, string) ExtractTools()
        {
            var temppath = Path.GetTempPath();

            RemoveTemp();

            Directory.CreateDirectory(temppath + TempCompressedTools);

            Directory.CreateDirectory(temppath + TempTools);

            // extract zipped files from application

            var steamemu_path = temppath + TempCompressedTools + "\\steamemu.zip";

            try
            {
                File.WriteAllBytes(steamemu_path, Properties.Resources.steamemu);
            }
            catch (Exception e)
            {
                return (false, "Failed save steamemu.zip in " + steamemu_path + " (" + e.Message + ")");
            }

            var steamstub_patcher_path = temppath + TempCompressedTools + "\\steamstub_patcher.zip";
            try
            {
                File.WriteAllBytes(steamstub_patcher_path, Properties.Resources.steamstub_patcher);
            }
            catch (Exception e)
            {
                return (false, "Failed save steamstub_patcher.zip in " + steamstub_patcher_path + " (" + e.Message + ")");
            }

            // extract files from zipper files

            try
            {
                ZipFile.ExtractToDirectory(steamemu_path, temppath + TempTools);
            }
            catch (Exception e)
            {
                return (false, "Failed extract steamemu.zip in " + steamemu_path + " (" + e.Message + ")");
            }

            try
            {
                ZipFile.ExtractToDirectory(steamstub_patcher_path, temppath + TempTools);
            }
            catch (Exception e)
            {
                return (false, "Failed extract steamstub_patcher.zip in " + steamstub_patcher_path + " (" + e.Message + ")");
            }

            ExtractedTools = true;

            return (true, "");
        }

        public void RemoveTemp()
        {
            var temppath = Path.GetTempPath();
            if (Directory.Exists(temppath + TempCompressedTools))
            {
                Directory.Delete(temppath + TempCompressedTools, true);
            }
            if (Directory.Exists(temppath + TempTools))
            {
                Directory.Delete(temppath + TempTools, true);
            }
        }

        public bool CheckSteamAPIBak()
        {
            var state = File.Exists(SteamAPIPath + ".bak"); // check bak of steamapi
            HasSteamAPIBak = state;
            return state;
        }

        public (bool, string) BackupRealSteamAPI()
        {
            if (!File.Exists(SteamAPIPath))
            {
                return (false, "Not real SteamAPI in /Binaries");
            }

            if (!HasSteamAPIBak) 
            {
                try
                {
                    File.Copy(SteamAPIPath, SteamAPIPath + ".bak");
                }
                catch (Exception e)
                {
                    return (false, "Failed save SteamAPI backup in /Binaries (" + e.Message + ")");
                }
            }

            return (true, "");
        }
    }
}
