using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TROTDS
{
    public enum MethodType
    {
        None,

        GamePathUpdate,
        GameProcessStateUpdate,
        ValidatePatchedExecutable,
        SteamEmuDataUpdate,
        ProcessWatchError,

        FullPatch,
        FullRestore,
        LocatingGamePathRequest,
        LocatingGamePathResult,
        PlayPatched,
    }
}
