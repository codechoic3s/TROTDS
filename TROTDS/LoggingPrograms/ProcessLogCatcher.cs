using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TROTDS.Logging;

namespace TROTDS.LoggingPrograms
{
    public class ProcessLogCatcher
    {
        private List<string> _logs;
        public Process ProgramProcess;
        public string SavePath;
        public ProcessLogCatcher(string savePath)
        {
            SavePath = savePath;
            _logs = new List<string>();
        }

        private void Log(string message)
        {
            _logs.Add(message);
        }

        public void Clear()
        {
            _logs.Clear();
        }

        public string CollectLogs()
        {
            string str = "";

            var lco = _logs.Count;

            for (int i = 0; i < lco; i++)
            {
                str += _logs[i] + "\n";
            }

            return str;
        }

        public bool Save(LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            methodExecutor.Add(() => { return Utils.TryFileWriteAllText(SavePath, CollectLogs(), logTask); });

            return methodExecutor.Execute();
        }

        public bool RunProgram(string programPath, string args, LogTask logTask = null)
        {
            MethodExecutor methodExecutor = new MethodExecutor();

            // setup process
            ProgramProcess = new Process();

            var processInfoStart = new ProcessStartInfo(programPath, args);
            processInfoStart.WindowStyle = ProcessWindowStyle.Hidden;
            processInfoStart.CreateNoWindow = true;
            processInfoStart.RedirectStandardOutput = true;
            processInfoStart.RedirectStandardError = true;
            processInfoStart.UseShellExecute = false;

            ProgramProcess.StartInfo = processInfoStart;

            ProgramProcess.OutputDataReceived += ProgramProcess_OutputDataReceived;
            ProgramProcess.ErrorDataReceived += ProgramProcess_ErrorDataReceived;

            // setup execution

            methodExecutor.Add(() => { 
                return Utils.TryProcessStart(ProgramProcess, logTask); });
            methodExecutor.Add(() => { 
                return Utils.TryProcessWaitForExit(ProgramProcess, 10000, logTask); });
            return methodExecutor.Execute();
        }

        private bool WaitProcess(int ms, LogTask logTask = null)
        {
            var state = ProgramProcess.WaitForExit(ms);
            logTask.Log(state ? ("Success of execute program " + ProgramProcess.ProcessName) : "Failed execute program " + ProgramProcess.ProcessName);
            return state;
        }

        private void ProgramProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log("(error) " + e.Data);
        }

        private void ProgramProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Log("(log) " + e.Data);
        }
    }
}
