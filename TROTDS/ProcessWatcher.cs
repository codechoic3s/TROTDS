using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TROTDS.Logging;

namespace TROTDS
{
    public class ProcessWatcher
    {
        public string ProcessNameFind;
        //public string ProcessTitleNameFind;
        //public string ProcessTitleNameFindCorrupted;

        public ProcessWatcher(string processNameFind, LogTask logTask = null)
        {
            ProcessNameFind = processNameFind;
            //ProcessTitleNameFind = processTitleNameFind;
            LogTask = logTask;
        }

        private LogTask LogTask;
        public Process OsProcess { get; private set; }
        private Thread WatchingThread;
        public bool Watching { get; private set; }
        public Action<bool> OnProcessState { get; set; }
        public void BeginWatch()
        {
            if (!Watching)
            {
                WatchingThread = new Thread(WatchingMethod);
                WatchingThread.Start();
            }
        }
        public void EndWatch()
        {
            Watching = false;
        }
        
        private void WatchingMethod()
        {
            Watching = true;
            Utils.TrySafeWork("ProcessWatcher", () =>
            {
                while (Watching)
                {
                    if (!(OsProcess is null))
                    {
                        var newstate = Utils.TryProcessWaitForExit(OsProcess, 1000, LogTask);

                        if (newstate != true) continue; // skip if process not ended

                        OsProcess = null; // invalidate old process data

                        Utils.TrySafeWork("ProcessWatcher.Callback", () => { OnProcessState?.Invoke(newstate); }, LogTask);
                    }
                    else
                    {
                        var processes = Process.GetProcesses();
                        var pco = processes.LongLength;
                        for (var i = 0; i < pco; i++)
                        {
                            var process = processes[i];
                            if (!process.ProcessName.Contains(ProcessNameFind)) continue; // skip if not contains

                            OsProcess = process; // finded
                            Utils.TrySafeWork("ProcessWatcher.Callback", () => { OnProcessState?.Invoke(false); }, LogTask);
                            break;
                        }
                    }
                    Thread.Sleep(1000);
                }
            }, LogTask);
            Watching = false;
        }
    }
}
