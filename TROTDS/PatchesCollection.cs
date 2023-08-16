using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using TROTDS.Logging;
using TROTDS.LoggingPrograms;
using TROTDS.PatchSupport;

namespace TROTDS
{
    public class PatchesCollection
    {
        public PatchesCollection()
        {
            
        }

        public static bool ValidatePath(string gamePath, Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor(true);

            pathes.GamePath = gamePath;

            methodExecutor.Add(() => { return Utils.TryDirectoryExists(pathes.GamePath, logTask); });

            pathes.GameBinariesPath = pathes.GamePath + "\\Binaries";

            methodExecutor.Add(() => { return Utils.TryDirectoryExists(pathes.GameBinariesPath, logTask); });

            pathes.CoalescedGamePath = pathes.GamePath + "\\TransGame\\Config\\PC\\Cooked\\Coalesced.ini";

            methodExecutor.Add(() => { return Utils.TryFileExists(pathes.CoalescedGamePath, logTask); });

            pathes.SteamEmuSettingsPath = pathes.GameBinariesPath + "\\settings";

            pathes.GameExecutablePath = pathes.GamePath + "\\Binaries\\TransGame.exe";

            methodExecutor.Add(() => { return Utils.TryFileExists(pathes.GameExecutablePath, logTask); });

            pathes.PatchedGameExecutablePath = pathes.GameBinariesPath + "\\TransGame.exe.unpacked.exe";
            pathes.HasPatchedExecutablePath = Utils.TryFileExists(pathes.PatchedGameExecutablePath, logTask);

            pathes.SteamAPIPath = pathes.GamePath + "\\Binaries\\steam_api.dll";
            methodExecutor.Add(() => { return Utils.TryFileExists(pathes.SteamAPIPath, logTask); });

            var newstate = methodExecutor.Execute();

            pathes.VerifiedGamePath = newstate ? true : pathes.VerifiedGamePath;

            return newstate;
        }
        public static bool FullPatch(string gamePath, Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => {
                return ValidatePath(gamePath, pathes, logTask); });

            methodExecutor.Add(() => { 
                return SteamAPI.BackupSteamAPI(pathes, logTask); });

            methodExecutor.Add(() => { 
                return GameConfig.BackupConfig(pathes, logTask); });

            methodExecutor.Add(() => { 
                return ExtraTools.ExtractTools(pathes, logTask); });

            methodExecutor.Add(() => { 
                return SteamStub.PatchGameExecutable(pathes, logTask); });

            methodExecutor.Add(() => { 
                return SteamAPI.LoadCustomSteamAPI(pathes, logTask); });

            methodExecutor.Add(() => { 
                return ExtraTools.RemoveTools(pathes, logTask); });

            methodExecutor.Add(() => { 
                return GameConfig.ReplaceConfig(pathes, logTask); });

            return methodExecutor.Execute();
        }

        public static bool FullRestore(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { 
                return SteamAPI.RestoreSteamAPI(pathes, logTask); });

            methodExecutor.Add(() => { 
                return GameConfig.RestoreConfig(pathes, logTask); });

            methodExecutor.Add(() => {
                pathes.HasPatchedExecutablePath = false;
                return SteamStub.RemovePatchedGameExecutable(pathes, logTask); });

            return methodExecutor.Execute();
        }

        public static bool CheckExecutables(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { return pathes.HasPatchedExecutablePath = Utils.TryFileExists(pathes.PatchedGameExecutablePath, logTask); });

            return methodExecutor.Execute();
        }

        public static bool TryRunPatched(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { 
                return pathes.HasPatchedExecutablePath = Utils.TryFileExists(pathes.PatchedGameExecutablePath, logTask); });

            methodExecutor.Add(() => { 
                return Utils.TryProcessStart(new Process() { StartInfo = new ProcessStartInfo(pathes.PatchedGameExecutablePath) }, logTask); });

            return methodExecutor.Execute();
        }

        public static bool ChangeSteamEmuNick(string nick, Pathes pathes, LogTask logTask = null)
        {
            return SteamAPI.ChangeNick(nick, pathes, logTask);
        }

        public static bool ChangeSteamEmuLang(SteamEmuLang lang, Pathes pathes, LogTask logTask = null)
        {
            return SteamAPI.ChangeLang(lang, pathes, logTask);
        }
    }
}
