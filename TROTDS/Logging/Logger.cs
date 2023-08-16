using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS.Logging
{
    public class Logger : LogTask
    {
        public Logger()
        {
            
        }

        public override void Log(string message)
        {
            _logs.Add(message);
            OnLog?.Invoke(message);
        }

        public void Save()
        {
            Utils.TryFileWriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\log.txt", CollectLogs());
        }
    }
}
