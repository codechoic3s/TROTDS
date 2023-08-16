using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TROTDS.Logging;

namespace TROTDS.PatchSupport
{
    public static class ExtraTools
    {
        public static bool ExtractTools(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor(true, MethodExecutionType.All);

            methodExecutor.Add(() => { 
                return RemoveTools(pathes, logTask); });
            methodExecutor.Add(() => { 
                return Utils.TryCreateDirectory(pathes.TempCompressedTools, logTask); });
            methodExecutor.Add(() => { 
                return Utils.TryCreateDirectory(pathes.TempTools, logTask); });

            // extract zipped files from application

            var steamemu_path = pathes.TempCompressedTools + "\\steamemu.zip";

            methodExecutor.Add(() => { 
                return Utils.TryFileWriteAllBytes(steamemu_path, Properties.Resources.steamemu, logTask); });

            var steamstub_patcher_path = pathes.TempCompressedTools + "\\steamstub_patcher.zip";

            methodExecutor.Add(() => { 
                return Utils.TryFileWriteAllBytes(steamstub_patcher_path, Properties.Resources.steamstub_patcher, logTask); });

            // extract files from zipped files
            methodExecutor.Add(() => { 
                return Utils.TryZipFileExtractToDirectory(steamemu_path, pathes.TempTools, logTask); });
            methodExecutor.Add(() => { 
                return Utils.TryZipFileExtractToDirectory(steamstub_patcher_path, pathes.TempTools, logTask); });

            return methodExecutor.Execute();
        }

        public static bool RemoveTools(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor(true, MethodExecutionType.All);

            methodExecutor.Add(() => { return Utils.TryDeleteDirectory(pathes.TempCompressedTools, logTask); }); // remove zipped
            methodExecutor.Add(() => { return Utils.TryDeleteDirectory(pathes.TempTools, logTask); }); // remove raw files

            return methodExecutor.Execute();
        }
    }
}
