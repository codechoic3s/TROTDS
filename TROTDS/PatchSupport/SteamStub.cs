using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TROTDS.Logging;
using TROTDS.LoggingPrograms;

namespace TROTDS.PatchSupport
{
    public static class SteamStub
    {
        public static bool PatchGameExecutable(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor(true);

            var steamstub_path = pathes.TempTools + "\\steamstub_patcher\\Steamless.CLI.exe";

            var plc = new ProcessLogCatcher(pathes.NearPath + "\\steamstub.txt");

            var args = $"--realign {'"'}{pathes.GameExecutablePath}{'"'}";

            methodExecutor.Add(() => { 
                return Utils.TryFileExists(pathes.GameExecutablePath, logTask); }); // verify original

            methodExecutor.Add(() => { 
                return plc.RunProgram(steamstub_path, args, logTask); }); // try patch
            methodExecutor.Add(() => { 
                return plc.Save(logTask); }); // save patch log

            methodExecutor.Add(() => { 
                return pathes.HasPatchedExecutablePath = Utils.TryFileExists(pathes.PatchedGameExecutablePath, logTask); }); // verify patched

            return methodExecutor.Execute();
        }

        public static bool RemovePatchedGameExecutable(Pathes pathes, LogTask logTask = null)
        {
            return Utils.TryDeleteFile(pathes.PatchedGameExecutablePath, logTask);
        }
    }
}
