using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TROTDS.Logging;
using TROTDS.PatchSupport;
using TROTDS.Windows;

namespace TROTDS
{
    public class MainApp
    {
        private Thread windowThread;
        private TROTDSLauncher launcherWindow;
        
        public PatchesCollection PatchesCollection { get; private set; }
        public SettingsService SettingsService { get; private set; }

        public Logger Logger { get; private set; }
        public AutoPatcher AutoPatcher { get; private set; }

        public void Start()
        {
            SettingsService = new SettingsService();
            SettingsService.Start();
            Logger = new Logger();
            AutoPatcher = new AutoPatcher(this);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            windowThread = new Thread(WindowStart);
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Save();
        }

        private void WindowStart(object args)
        {
            PatchesCollection = new PatchesCollection();

            launcherWindow = new TROTDSLauncher(this);
            launcherWindow.ShowDialog();
        }
    }
}
