using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TROTDS.Logging;

namespace TROTDS.PatchSupport
{
    public static class GameConfig
    {
        public static bool BackupConfig(Pathes pathes, LogTask logTask = null)
        {
            return Utils.TryFileExists(pathes.CoalescedGamePath + ".bak", logTask) || Utils.TryCopyFile(pathes.CoalescedGamePath, pathes.CoalescedGamePath + ".bak", logTask);
        }

        public static bool RestoreConfig(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { return Utils.TryFileExists(pathes.CoalescedGamePath + ".bak", logTask); });

            methodExecutor.Add(() => { return Utils.TryDeleteFile(pathes.CoalescedGamePath, logTask); });
            methodExecutor.Add(() => { return Utils.TryCopyFile(pathes.CoalescedGamePath + ".bak", pathes.CoalescedGamePath, logTask); });
            methodExecutor.Add(() => { return Utils.TryDeleteFile(pathes.CoalescedGamePath + ".bak", logTask); });

            return methodExecutor.Execute();
        }

        public static bool ReplaceConfig(Pathes pathes, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { return Utils.TryFileWriteAllBytes(pathes.CoalescedGamePath, Properties.Resources.coalesced, logTask); });
            
            return methodExecutor.Execute();
        }
    }
}
