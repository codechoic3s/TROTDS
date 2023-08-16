using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TROTDS.Logging;

namespace TROTDS.PatchSupport
{
    public static class SteamAPI
    {
        public static bool BackupSteamAPI(Pathes pathes, LogTask logTask = null)
        {
            return Utils.TryFileExists(pathes.SteamAPIPath + ".bak", logTask) || Utils.TryCopyFile(pathes.SteamAPIPath, pathes.SteamAPIPath + ".bak", logTask);
        }
        public static bool RestoreSteamAPI(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor(true, MethodExecutionType.All);

            methodExecutor.Add(() => { 
                return Utils.TryFileExists(pathes.SteamAPIPath + ".bak", logTask); }); // restore from bak

            string[] files = new string[4] { "steam_api.dll", "local_save.txt", "steam_appid.txt", "steam_interfaces.txt" };

            long ico = files.LongLength;
            for (long i = 0; i < ico; i++)
            {
                string fileName = files[i];
                methodExecutor.Add(() => { 
                    return Utils.TryDeleteFile(pathes.GameBinariesPath + "\\" + fileName, logTask); });
            }

            methodExecutor.Add(() => { 
                return Utils.TryDeleteDirectory(pathes.GameBinariesPath + "\\settings", logTask); }); // remove settings folder

            methodExecutor.Add(() => { 
                return Utils.TryCopyFile(pathes.SteamAPIPath + ".bak", pathes.SteamAPIPath, logTask); }); // restore from bak

            methodExecutor.Add(() => {
                return Utils.TryDeleteFile(pathes.SteamAPIPath + ".bak", logTask); }); // remove bak

            return methodExecutor.Execute();
        }
        public static bool LoadCustomSteamAPI(Pathes pathes, LogTask logTask = null)
        {
            string steamemu_path = pathes.TempTools + "\\steamemu";

            MethodExecutor methodExecutor = new MethodExecutor();

            string[] files = new string[4] { "steam_api.dll", "local_save.txt", "steam_appid.txt", "steam_interfaces.txt" };

            long ico = files.LongLength;
            for (long i = 0; i < ico; i++)
            {
                string fileName = files[i];
                methodExecutor.Add(() => { return Utils.TryDeleteFile(pathes.GameBinariesPath + "\\" + fileName, logTask); });
                methodExecutor.Add(() => { return Utils.TryCopyFile(steamemu_path + "\\" + fileName, pathes.GameBinariesPath + "\\" + fileName, logTask); });
            }

            return methodExecutor.Execute();
        }
        public static bool ChangeLang(SteamEmuLang lang, Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { return Utils.TryCreateDirectory(pathes.SteamEmuSettingsPath, logTask); });
            methodExecutor.Add(() => { return Utils.TryFileWriteAllText(pathes.SteamEmuSettingsPath + "\\language.txt", lang.ToString(), logTask); });

            return methodExecutor.Execute();
        }
        public static bool ChangeNick(string nick, Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { return Utils.TryCreateDirectory(pathes.SteamEmuSettingsPath, logTask); });
            methodExecutor.Add(() => { return Utils.TryFileWriteAllText(pathes.SteamEmuSettingsPath + "\\account_name.txt", nick, logTask); });

            return methodExecutor.Execute();
        }
    }
}
