using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public (bool, string) Save()
        {
            try
            {
                File.WriteAllText(SavePath, CollectLogs());
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

            return (true, "");
        }

        public bool RunProgram(string programPath, string args)
        {
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

            return ProgramProcess.Start();
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
