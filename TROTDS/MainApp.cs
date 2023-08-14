using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TROTDS
{
    public class MainApp
    {
        private Thread windowThread;
        private TROTDSLauncher launcherWindow;
        
        public GameManager GameManager;
        public SettingsService SettingsService;
        public void Start()
        {
            SettingsService = new SettingsService();
            SettingsService.Start();

            windowThread = new Thread(WindowStart);
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();
        }

        private void WindowStart(object args)
        {
            GameManager = new GameManager();

            launcherWindow = new TROTDSLauncher(this);
            launcherWindow.ShowDialog();
        }
    }
}
