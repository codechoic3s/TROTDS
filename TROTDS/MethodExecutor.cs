using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS
{
    public class MethodExecutor
    {
        public bool Locked { get; private set; }
        public bool IgnoreFail { get; set; }
        public MethodExecutionType ExecutionType { get; set; }
        private List<Func<bool>> ExecutionList;
        public MethodExecutor(bool ignoreFail = false, MethodExecutionType executionType = MethodExecutionType.AllTrue)
        {
            ExecutionList = new List<Func<bool>>();
            Locked = false;
            IgnoreFail = ignoreFail;
            ExecutionType = executionType;
        }
        public void Add(Func<bool> func)
        {
            if (Locked) return;
            ExecutionList.Add(func);
        }
        public bool Execute()
        {
            Locked = true;
            bool ok = true;
            int elco = ExecutionList.Count;
            for (int i = 0; i < elco; i++)
            {
                if (ExecutionList[i].Invoke()) continue; // if ok go to next

                ok = false;

                if (!IgnoreFail) 
                    return ok; // force return
            }
            ExecutionList.Clear();
            Locked = false;

            switch (ExecutionType)
            {
                default: return false;
                case MethodExecutionType.AllTrue: return ok == true;
                case MethodExecutionType.AllFalse: return ok == false;
                case MethodExecutionType.All: return true;
            }
        }
    }
}
