using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS.Logging
{
    public class LogTask
    {
        public Logger Logger { get; set; }
        public Action<string> OnLog;

        protected internal List<string> _logs;

        public LogTask(Logger logger = null)
        {
            Logger = logger;
            _logs = new List<string>();
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
        public virtual void Log(string message)
        {
            _logs.Add(message);
            Logger?.Log(message);
            OnLog?.Invoke(message);
        }
    }
}
