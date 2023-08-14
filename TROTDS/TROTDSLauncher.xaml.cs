using System;
using System.Collections.Generic;
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

namespace TROTDS
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
            Loaded += TROTDSLauncher_Loaded;
            InitializeComponent();
        }

        private void TROTDSLauncher_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MainApp.SettingsService.Config.GamePath))
            {
                currentGamePath_label.Content = "Game path not loaded!";
            }

            var state = MainApp.GameManager.ValidatePath(MainApp.SettingsService.Config.GamePath);

            if (!state.Item1)
            {
                System.Windows.Forms.MessageBox.Show("Not valid path to game from config!!!\n\n" + state.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                currentGamePath_label.Content = "Game path: " + MainApp.SettingsService.Config.GamePath;
            }

            startPatched_btn.IsEnabled = MainApp.GameManager.HasPatchedExecutablePath;

            nickName_tb.Text = MainApp.SettingsService.Config.GameNick;

            gameLang_combobox.ItemsSource = Enum.GetValues(typeof(SteamEmuLang));
        }

        private void fullPatch_btn_Click(object sender, RoutedEventArgs e)
        {
            var state = MainApp.GameManager.FullPatch(MainApp.SettingsService.Config.GamePath);

            if (!state.Item1)
            {
                System.Windows.Forms.MessageBox.Show(state.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            startPatched_btn.IsEnabled = MainApp.GameManager.HasPatchedExecutablePath;

            System.Windows.Forms.MessageBox.Show("Game patched!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void locateGamePath_btn_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.MessageBox.Show("Not selected path with game!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var state = MainApp.GameManager.ValidatePath(fbd.SelectedPath);

            if (!state.Item1)
            {
                System.Windows.Forms.MessageBox.Show(state.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MainApp.SettingsService.Config.GamePath = fbd.SelectedPath;
            MainApp.SettingsService.Save();

            currentGamePath_label.Content = "Game path: " + MainApp.SettingsService.Config.GamePath;
        }

        private void startPatched_btn_Click(object sender, RoutedEventArgs e)
        {
            var state = MainApp.GameManager.ChangeGameLang(MainApp.SettingsService.Config.GameLang);
            if (!state.Item1)
            {
                System.Windows.Forms.MessageBox.Show(state.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            state = MainApp.GameManager.ChangeGameNick(MainApp.SettingsService.Config.GameNick);
            if (!state.Item1)
            {
                System.Windows.Forms.MessageBox.Show(state.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            state = MainApp.GameManager.TryRunPatched();
            if (!state.Item1)
            {
                System.Windows.Forms.MessageBox.Show(state.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Windows.Forms.MessageBox.Show("Параша запущена!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void gameLang_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((SteamEmuLang)gameLang_combobox.SelectedItem == SteamEmuLang.none)
            {
                gameLang_combobox.SelectedIndex = (int)MainApp.SettingsService.Config.GameLang;
                return;
            }
            MainApp.SettingsService.Config.GameLang = (SteamEmuLang)gameLang_combobox.SelectedIndex;
            MainApp.SettingsService.Save();
        }

        private void nickName_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            MainApp.SettingsService.Config.GameNick = nickName_tb.Text;
            MainApp.SettingsService.Save();
        }
    }
}
