using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TROTDS.ConfigsStructs;
using static System.Windows.Forms.AxHost;

namespace TROTDS.Windows
{
    /// <summary>
    /// Interaction logic for TROTDSLauncher.xaml
    /// </summary>
    public partial class TROTDSLauncher : Window
    {
        public MainApp MainApp;
        public TROTDSLauncher(MainApp mp)
        {
            MainApp = mp;
            MainApp.AutoPatcher.OnState = OnState; // link ui
            Loaded += TROTDSLauncher_Loaded;
            Closing += TROTDSLauncher_Closing;
            InitializeComponent();
        }

        private void TROTDSLauncher_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainApp.Logger.OnLog = null;
            MainApp.AutoPatcher.StopAction();
        }

        private object OnState(MethodType mt, bool state, object data)
        {
            switch(mt)
            {
                default: return null;
                case MethodType.FullPatch: OnFullPatch(state, (string)data); return null;
                case MethodType.FullRestore: OnFullRestore(state, (string)data); return null;
                case MethodType.LocatingGamePathRequest: return OnLocatingGamePathRequest();
                case MethodType.LocatingGamePathResult: OnLocatingGamePathResult(state, (string)data); return null;
                case MethodType.PlayPatched: OnPlayPatched(state, (string)data); return null;

                case MethodType.GamePathUpdate: OnGamePathUpdate(state, (string)data); return null;
                case MethodType.GameProcessStateUpdate: OnGameProcessStateUpdate(state); return null;
                case MethodType.ValidatePatchedExecutable: OnValidatePatchedExecutable(state); return null;
                case MethodType.SteamEmuDataUpdate: OnSteamEmuDataUpdate((SteamEmuConfig)data); return null;
                case MethodType.ProcessWatchError: OnProcessWatchError(state, (string)data); return null;
            }
        }
        private void OnGamePathUpdate(bool state, string text)
        {
            HandleMessageBox(text, state, true);
            if (!state) return; // stop on error

            Dispatcher.Invoke(() =>
            {
                currentGamePath_label.Content = text; // show new path
            });
        }
        private bool ProcessState;
        private void OnGameProcessStateUpdate(bool state)
        {
            ProcessState = !state;
            Dispatcher.Invoke(() =>
            {
                startPatched_btn.Content = ProcessState ? "Kill" : "Run patched";
            });
        }
        private void OnValidatePatchedExecutable(bool state)
        {
            Dispatcher.Invoke(() =>
            {
                startPatched_btn.IsEnabled = state || ProcessState;
            });
        }
        private void OnSteamEmuDataUpdate(SteamEmuConfig config)
        {
            Dispatcher.Invoke(() =>
            {
                nickName_tb.Text = config.Nick;
                LastSteamEmuLang = config.Lang;
            });
        }
        private void OnProcessWatchError(bool state, string log)
        {
            HandleMessageBox(log, state, true);
            if (!state) return; // stop on error
        }

        private void OnFullPatch(bool state, string log)
        {
            HandleMessageBox(log, state);
            if (!state) return; // stop on error
        }
        private void OnFullRestore(bool state, string log)
        {
            HandleMessageBox(log, state);
            if (!state) return; // stop on error
        }
        private object OnLocatingGamePathRequest()
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.MessageBox.Show("Not selected path with game!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return fbd.SelectedPath;
        }
        private void OnLocatingGamePathResult(bool state, string log)
        {
            HandleMessageBox(log, state);
            if (!state) return; // stop on error
        }
        private void OnPlayPatched(bool state, string log)
        {
            HandleMessageBox(log, state, true);
            if (!state) return; // stop on error

        }
        private void HandleMessageBox(string message, bool isOk, bool onlyErrors = false)
        {
            string title = "Error";
            MessageBoxImage icon = MessageBoxImage.Error;

            if (isOk)
            {
                title = "Information";
                icon = MessageBoxImage.Information;

                if (onlyErrors) return;
            }

            System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }
        private void LogListBox(string log)
        {
            Dispatcher.Invoke(() =>
            {
                logs_listbox.Items.Add(log);
            });
        }
        private void TROTDSLauncher_Loaded(object sender, RoutedEventArgs e)
        {
            MainApp.Logger.OnLog = LogListBox;

            MainApp.AutoPatcher.StartAction();

            gameLang_combobox.ItemsSource = Enum.GetValues(typeof(SteamEmuLang));
        }

        private void fullPatch_btn_Click(object sender, RoutedEventArgs e)
        {
            MainApp.AutoPatcher.FullPatch();
        }

        private void locateGamePath_btn_Click(object sender, RoutedEventArgs e)
        {
            MainApp.AutoPatcher.LocateGamePath();
        }

        private void startPatched_btn_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessState)
            {
                MainApp.AutoPatcher.KillProcess();
                return;
            }
            MainApp.AutoPatcher.PlayPatched();
        }
        private SteamEmuLang LastSteamEmuLang;
        private void gameLang_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((SteamEmuLang)gameLang_combobox.SelectedItem == SteamEmuLang.none)
            {
                gameLang_combobox.SelectedIndex = (int)LastSteamEmuLang;
                return;
            }
            MainApp.AutoPatcher.SetSteamEmuLang((SteamEmuLang)gameLang_combobox.SelectedIndex);
        }

        private void nickName_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainApp.AutoPatcher.SetSteamEmuNick(nickName_tb.Text);
        }

        private void restore_btn_Click(object sender, RoutedEventArgs e)
        {
            MainApp.AutoPatcher.FullRestore();
        }

        private void link_label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainApp.AutoPatcher.OpenUrl();
        }

        private void currentGamePath_label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainApp.AutoPatcher.OpenAppFolder();
        }
    }
}
