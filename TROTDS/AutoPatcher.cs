using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Diagnostics;

namespace TROTDS
{
    public class AutoPatcher
    {
        public Pathes Pathes { get; private set; }
        public PatchesCollection PatchesCollection { get; private set; }
        public MainApp MainApp { get; private set; }
        public ProcessWatcher ProcessWatcher { get; private set; }
        public AutoPatcher(MainApp mainApp)
        {
            MainApp = mainApp;
            Pathes = new Pathes();
            PatchesCollection = new PatchesCollection();
            ProcessWatcher = new ProcessWatcher("TransGame", MainApp.Logger);
            ProcessWatcher.OnProcessState = OnProcessState;
        }
        public Func<MethodType, bool, object, object> OnState { get; set; }

        private void OnProcessState(bool state)
        {
            OnState?.Invoke(MethodType.GameProcessStateUpdate, state, null);
        }

        public void StartAction()
        {
            ProcessWatcher.BeginWatch();

            // get game path from config.
            if (!string.IsNullOrEmpty(MainApp.SettingsService.Config.Core.AppPath) && !PatchesCollection.ValidatePath(MainApp.SettingsService.Config.Core.AppPath, Pathes, MainApp.Logger))
            {
                OnState?.Invoke(MethodType.GamePathUpdate, false, "Not a real game!");
                OnState?.Invoke(MethodType.ValidatePatchedExecutable, Pathes.HasPatchedExecutablePath, null);
                return;
            }

            OnState?.Invoke(MethodType.GamePathUpdate, true, MainApp.SettingsService.Config.Core.AppPath);

            OnState?.Invoke(MethodType.ValidatePatchedExecutable, Pathes.HasPatchedExecutablePath, null);

            OnState?.Invoke(MethodType.SteamEmuDataUpdate, true, MainApp.SettingsService.Config.SteamEmu);

        }
        public void StopAction()
        {
            ProcessWatcher.EndWatch();

        }
        public void OpenUrl()
        {
            Utils.TryProcessStart( new Process() { StartInfo = new ProcessStartInfo("https://github.com/codechoic3s/TROTDS") });
        }
        public void OpenAppFolder()
        {
            if (!Pathes.VerifiedGamePath) return;
            Utils.TryProcessStart(new Process() { StartInfo = new ProcessStartInfo(MainApp.SettingsService.Config.Core.AppPath) });
        }
        public void FullPatch()
        {
            var state = PatchesCollection.FullPatch(MainApp.SettingsService.Config.Core.AppPath, Pathes, MainApp.Logger);
            var msg = state ? "Success patched!" : "Failed full patch!";
            OnState?.Invoke(MethodType.FullPatch, state, msg);
            OnState?.Invoke(MethodType.ValidatePatchedExecutable, Pathes.HasPatchedExecutablePath, null);
        }
        public void LocateGamePath()
        {
            var pathObject = OnState?.Invoke(MethodType.LocatingGamePathRequest, false, "");
            if (!(pathObject is string pathString))
            {
                OnState?.Invoke(MethodType.LocatingGamePathResult, false, "Expected string!");
                return;
            }

            var state = PatchesCollection.ValidatePath(pathString, Pathes, MainApp.Logger);

            if (state) // save to config
            {
                MainApp.SettingsService.Config.Core.AppPath = pathString;
                MainApp.SettingsService.Save();
            }

            var msg = state ? "Correct game path!" : "Not correct game path!";
            OnState?.Invoke(MethodType.LocatingGamePathResult, state, msg);
            OnState?.Invoke(MethodType.GamePathUpdate, true, MainApp.SettingsService.Config.Core.AppPath);
        }
        public void FullRestore()
        {
            var state = PatchesCollection.FullRestore(Pathes, MainApp.Logger);
            var msg = state ? "Success restored!" : "Failed restore!";
            OnState?.Invoke(MethodType.FullRestore, state, msg);
            OnState?.Invoke(MethodType.ValidatePatchedExecutable, Pathes.HasPatchedExecutablePath, null);
        }

        public void PlayPatched()
        {
            if (!PatchesCollection.ChangeSteamEmuLang(MainApp.SettingsService.Config.SteamEmu.Lang, Pathes, MainApp.Logger))
            {
                OnState?.Invoke(MethodType.PlayPatched, false, "Failed change steamemu lang.");
                return;
            }

            if (!PatchesCollection.ChangeSteamEmuNick(MainApp.SettingsService.Config.SteamEmu.Nick, Pathes, MainApp.Logger))
            {
                OnState?.Invoke(MethodType.PlayPatched, false, "Failed change steamemu nick.");
                return;
            }

            var state = PatchesCollection.TryRunPatched(Pathes, MainApp.Logger);
            var msg = state ? "Started!" : "Failed start!";
            OnState?.Invoke(MethodType.PlayPatched, state, msg);
        }
        public void KillProcess()
        {
            if (ProcessWatcher.OsProcess is null)
            {
                OnState?.Invoke(MethodType.ProcessWatchError, false, "Not located process in system.");
                return;
            }
            if (!Utils.TryKillProcess(ProcessWatcher.OsProcess))
            {
                OnState?.Invoke(MethodType.ProcessWatchError, false, "Failed kill process.");
                return;
            }
            OnState?.Invoke(MethodType.ProcessWatchError, true, null);
        }

        public void SetSteamEmuNick(string nick)
        {
            MainApp.SettingsService.Config.SteamEmu.Nick = nick;
            MainApp.SettingsService.Save();
        }
        public void SetSteamEmuLang(SteamEmuLang lang)
        {
            MainApp.SettingsService.Config.SteamEmu.Lang = lang;
            MainApp.SettingsService.Save();
        }
    }
}
