using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroGUI.Desktop.Views.MainForm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class MainForm : Form
    {
        private static bool _isSafeShutdownAllowed = true;

        private static bool IsSafeShutdownAllowed {
            get { return _isSafeShutdownAllowed; }
            set { _isSafeShutdownAllowed = value; }
        }

        private Command CommandExport { get; set; }
        private Command CommandAccountUnlock { get; set; }
        private Command CommandAccountChangePassphrase { get; set; }

        private ButtonMenuItem MenuSettings { get; set; }

        private TabControl TabControlMain { get; set; }

        public MainForm()
        {
            this.SetWindowProperties(
                () => Desktop.Properties.Resources.TextClientName,
                new Size(850, 570),
                new Size(640, 480)
            );
            this.SetLocationToCenterScreen();

            Shown += delegate { InitializeCoreApi(); };
            Closed += OnFormClosed;

            Utilities.Initialize(this);

            //var clipboardText = "";

            //clipboardText +=
            //    "Configuration file: " + Path.Combine(
            //        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            //        Utilities.GetAssemblyAttribute<System.Reflection.AssemblyCompanyAttribute>().Company,
            //        Utilities.ApplicationAssemblyName.Name,
            //        "user.config"
            //    ) + "\r\n" +
            //    "Daemon data dir: " + SettingsManager.Paths.DirectoryDaemonData + "\r\n" +
            //    "Account data file: " + SettingsManager.Paths.FileAccountData + "\r\n" +
            //    "Default account data directory: " + MoneroAPI.Extensions.Utilities.DefaultPathDirectoryAccountData + "\r\n" +
            //    "Default account data directory (test #2): " + Path.Combine(MoneroAPI.Extensions.Utilities.DefaultPathDirectoryAccountData, "MoneroX") + "\r\n" +
            //    "Default account data file: " + MoneroAPI.Extensions.Utilities.DefaultPathFileAccountData + "\r\n" +
            //    "Daemon software (relative path): " + SettingsManager.Paths.SoftwareDaemon + "\r\n" +
            //    "Daemon software (absolute path): " + Utilities.GetAbsolutePath(SettingsManager.Paths.SoftwareDaemon) + "\r\n" +
            //    "Executing assembly path: " + System.Reflection.Assembly.GetExecutingAssembly().Location + "\r\n" +
            //    "AppDomain base directory: " + AppDomain.CurrentDomain.BaseDirectory;

            //Utilities.Clipboard.Text = clipboardText;

            //Content = new TextArea {
            //    ReadOnly = true,
            //    Text = clipboardText
            //};

            RenderMenu();
            RenderContent();

#if !DEBUG
            // TODO: Add support for Mac
            if (Utilities.RunningPlatformId == PlatformID.Win32NT) {
                if (SettingsManager.General.IsUpdateCheckEnabled) {
                    Task.Factory.StartNew(CheckForUpdates);
                }
            }
#endif
        }

        static void OnFormClosed(object sender, EventArgs e)
        {
            var isSafeShutdownAllowed = IsSafeShutdownAllowed && SettingsManager.General.IsSafeShutdownEnabled;

            if (Utilities.MoneroRpcManager != null) {
                if (isSafeShutdownAllowed) {
                    Utilities.MoneroRpcManager.DisposeSafely();
                } else {
                    Utilities.MoneroRpcManager.Dispose();
                }
            }

            if (Utilities.MoneroProcessManager != null) {
                if (isSafeShutdownAllowed) {
                    Utilities.MoneroProcessManager.DisposeSafely();
                } else {
                    Utilities.MoneroProcessManager.Dispose();
                }
            }
        }

        private void CheckForUpdates()
        {
            using (var webClient = new WebClient()) {
                try {
                    // Compare the application's version with the latest one
                    var versionInfoString = webClient.DownloadString(new Uri("https://jojatekok.github.io/MoneroGui.Net/version_info/version_v1.txt", UriKind.Absolute));
                    var versionInfoStringSplit = versionInfoString.Split(new[] { '\n', '\t' });

                    var versionInfo = new Dictionary<string, string>();
                    for (var i = versionInfoStringSplit.Length - 1; i > 0; i -= 2) {
                        versionInfo.Add(versionInfoStringSplit[i - 1], versionInfoStringSplit[i]);
                    }

                    string latestVersionString;

                    if (!SettingsManager.General.IsUpdateCheckForTestBuildsEnabled) {
                        // Check for stable releases
                        latestVersionString = versionInfo["LatestVersionStable"];
                        var latestVersionComparable = new Version(latestVersionString + ".0");
                        if (latestVersionComparable.CompareTo(Utilities.ApplicationVersionComparable) <= 0) return;

                    } else {
                        // Check for experimental releases
                        latestVersionString = versionInfo["LatestVersionTest"];
                        var latestVersionStringSplit = latestVersionString.Split('-');
                        var latestVersionComparable = new Version(latestVersionStringSplit[0] + ".0");

                        if (latestVersionComparable.CompareTo(Utilities.ApplicationVersionComparable) <= 0) {
                            // Return if there is no "extra" version identifier
                            if (latestVersionStringSplit.Length == 1) return;

                            var latestVersionExtraSplit = latestVersionStringSplit[1].Split('.');
                            var applicationVersionExtraSplit = Utilities.ApplicationVersionExtra.Split('.');
                            if (applicationVersionExtraSplit.Length < 2) return;

                            if (latestVersionExtraSplit[0][0] <= applicationVersionExtraSplit[0][0] && byte.Parse(latestVersionExtraSplit[1]) <= byte.Parse(applicationVersionExtraSplit[1])) {
                                return;
                            }
                        }
                    }

                    var releasesUrlBase = versionInfo["ReleasesUrlBase"];
                    
                    var applicationBaseDirectory = Utilities.ApplicationBaseDirectory;

                    var updateName = "Update (v" + latestVersionString + ")";
                    var updatePath = applicationBaseDirectory + updateName;

                    // Check whether the update file has already been downloaded
                    if (!File.Exists(updatePath + ".zip")) {
                        var platformString = Utilities.GetRunningPlatformName();
                        var processorArchitectureString = Environment.Is64BitOperatingSystem ? "x64" : "x86";

                        webClient.DownloadFile(new Uri(string.Format(Utilities.InvariantCulture, releasesUrlBase, latestVersionString, platformString + "-" + processorArchitectureString), UriKind.Absolute), updatePath + ".zip");
                    }

                    Application.Instance.AsyncInvoke(() => {
                        // Check whether the user wants to apply the new update now
                        if (!this.ShowQuestion(string.Format(
                            Utilities.InvariantCulture,
                            Desktop.Properties.Resources.MainWindowUpdateQuestionMessage,
                            latestVersionString
                        ), Desktop.Properties.Resources.MainWindowUpdateQuestionTitle)) {
                            return;
                        }

                        // Extract the downloaded update
                        ZipFile.ExtractToDirectory(updatePath + ".zip", updatePath);

                        // Write a batch file which applies the update
                        using (var writer = new StreamWriter(applicationBaseDirectory + "Updater.bat")) {
                            var newLineString = Utilities.NewLineString;
                            writer.Write(
                                "XCOPY /S /V /Q /R /Y \"" + updateName + "\"" + newLineString +
                                "START \"\" \"" + Utilities.ApplicationAssemblyName.Name + ".exe\" --noupdate" + newLineString +
                                "RD /S /Q \"" + updateName + "\"" + newLineString +
                                "DEL /F /Q \"" + updateName + ".zip\"" + newLineString +
                                "DEL /F /Q %0"
                            );
                        }

                        new Process {
                            StartInfo = new ProcessStartInfo(applicationBaseDirectory + "Updater.bat") {
                                CreateNoWindow = true,
                                UseShellExecute = false
                            }
                        }.Start();

                        IsSafeShutdownAllowed = false;
                        Close();
                    });

                } catch {
                    Application.Instance.AsyncInvoke(() => this.ShowError(Desktop.Properties.Resources.MainWindowUpdateError));
                }
            }
        }

        void InitializeCoreApi()
        {
            var daemonRpc = Utilities.MoneroRpcManager.Daemon;
            var accountManagerRpc = Utilities.MoneroRpcManager.AccountManager;

            daemonRpc.NetworkInformationChanged += OnDaemonRpcNetworkInformationChanged;
            daemonRpc.BlockchainSynced += OnDaemonRpcBlockchainSynced;

            accountManagerRpc.Initialized += OnAccountManagerRpcInitialized;
            accountManagerRpc.AddressReceived += OnAccountManagerRpcAddressReceived;
            accountManagerRpc.TransactionReceived += OnAccountManagerRpcTransactionReceived;
            accountManagerRpc.TransactionChanged += OnAccountManagerRpcTransactionChanged;
            accountManagerRpc.BalanceChanged += OnAccountManagerRpcBalanceChanged;

            if (SettingsManager.Network.IsProcessDaemonHostedLocally) {
                // Initialize the daemon RPC manager as soon as the corresponding process is available
                var daemonProcess = Utilities.MoneroProcessManager.Daemon;
                daemonProcess.Initialized += delegate { daemonRpc.Initialize(); };
                daemonProcess.Start();
            } else {
                // Initialize the daemon RPC manager immediately
                daemonRpc.Initialize();
            }

            if (SettingsManager.Network.IsProcessAccountManagerHostedLocally) {
                // Initialize the account manager's RPC wrapper as soon as the corresponding process is available
                var accountManagerProcess = Utilities.MoneroProcessManager.AccountManager;
                accountManagerProcess.Initialized += delegate { accountManagerRpc.Initialize(); };
                accountManagerProcess.PassphraseRequested += OnAccountManagerProcessPassphraseRequested;
                accountManagerProcess.Start();
            } else {
                // Initialize the account manager's RPC wrapper immediately
                accountManagerRpc.Initialize();
            }
        }

        public void RenderMenu()
        {
            CommandExport = new Command(OnCommandExport) {
                MenuText = Desktop.Properties.Resources.MenuExport,
                Image = Utilities.LoadImage("Export"),
                Shortcut = Application.Instance.CommonModifier | Keys.E,
                Enabled = false
            };

            CommandAccountUnlock = new Command(OnCommandAccountUnlock) {
                MenuText = Desktop.Properties.Resources.MenuUnlockAccount,
                Image = Utilities.LoadImage("Key")
            };

            CommandAccountChangePassphrase = new Command(OnCommandAccountChangePassphrase) {
                MenuText = Desktop.Properties.Resources.MenuChangeAccountPassphrase,
                Image = Utilities.LoadImage("Key"),
                Enabled = false
            };

            var commandAccountBackupManager = new Command(OnCommandAccountBackupManager) {
                MenuText = Desktop.Properties.Resources.MenuBackupManager,
                Image = Utilities.LoadImage("Save"),
                Enabled = false
            };

            var commandExit = new Command(OnCommandExit) {
                MenuText = Desktop.Properties.Resources.MenuExit,
                Image = Utilities.LoadImage("Exit"),
                Shortcut = Application.Instance.CommonModifier | Keys.Q
            };

            var commandShowWindowOptions = new Command(OnCommandShowWindowOptions) {
                MenuText = Desktop.Properties.Resources.MenuOptions,
                Image = Utilities.LoadImage("Options"),
                Shortcut = Application.Instance.CommonModifier | Keys.O
            };

            var commandShowWindowAbout = new Command(OnCommandShowWindowAbout) {
                MenuText = Desktop.Properties.Resources.MenuAbout,
                Image = Utilities.LoadImage("Information"),
            };

            MenuSettings = new ButtonMenuItem {
                Text = Desktop.Properties.Resources.MenuSettings,
                Items = {
                    CommandAccountUnlock,
                    new SeparatorMenuItem(),
                    commandShowWindowOptions
                }
            };

            Menu = new MenuBar {
                Items = {
                    new ButtonMenuItem {
                        Text = Desktop.Properties.Resources.MenuFile,
                        Items = {
                            commandAccountBackupManager,
                            CommandExport,
                            new SeparatorMenuItem(),
                            commandExit
                        }
                    },

                    MenuSettings,

                    new ButtonMenuItem {
                        Text = Desktop.Properties.Resources.MenuHelp,
                        Items = {
                            commandShowWindowAbout
                        }
                    }
                }
            };
        }

        void RenderContent()
        {
            var tabPageOverview = new TabPage {
                Image = Utilities.LoadImage("Overview"),
                Content = new OverviewView()
            };
            tabPageOverview.SetTextBindingPath(() => " " + Desktop.Properties.Resources.MainWindowOverview);

            var tabPageSendCoins = new TabPage {
                Image = Utilities.LoadImage("Send"),
                Content = new SendCoinsView()
            };
            tabPageSendCoins.SetTextBindingPath(() => " " + Desktop.Properties.Resources.MainWindowSendCoins);

            var tabPageTransactions = new TabPage {
                Image = Utilities.LoadImage("Transaction"),
                Content = new TransactionsView()
            };
            tabPageTransactions.SetTextBindingPath(() => " " + Desktop.Properties.Resources.MainWindowTransactions);

            var tabPageAddressBook = new TabPage {
                Image = Utilities.LoadImage("Contact"),
                Content = new AddressBookView()
            };
            tabPageAddressBook.SetTextBindingPath(() => " " + Desktop.Properties.Resources.TextAddressBook);

            TabControlMain = new TabControl();
            TabControlMain.SelectedIndexChanged += OnTabControlMainSelectedIndexChanged;

            var tabControlPages = TabControlMain.Pages;
            tabControlPages.Add(tabPageOverview);
            tabControlPages.Add(tabPageSendCoins);
            tabControlPages.Add(tabPageTransactions);
            tabControlPages.Add(tabPageAddressBook);

            for (var i = tabControlPages.Count - 1; i >= 0; i--) {
                tabControlPages[i].Padding = new Padding(Utilities.Padding3);
            }

            Content = new TableLayout {
                Rows = {
                    new TableRow(
                        new Panel {
                            Padding = new Padding(Utilities.Padding4),
                            Content = TabControlMain
                        }
                    ) { ScaleHeight = true },

                    new TableRow(
                        new Panel {
                            BackgroundColor = Utilities.ColorStatusBar,
                            Padding = new Padding(Utilities.Padding4, Utilities.Padding2),
                            Content = new StatusBarView()
                        }
                    )
                }
            };
        }

        void OnTabControlMainSelectedIndexChanged(object sender, EventArgs e)
        {
            CommandExport.Enabled = TabControlMain.SelectedPage.Content is IExportable;
        }

        void OnCommandAccountBackupManager(object sender, EventArgs e)
        {
            
        }

        void OnCommandExport(object sender, EventArgs e)
        {
            var exportableContent = TabControlMain.SelectedPage.Content as IExportable;
            Debug.Assert(exportableContent != null, "exportableContent != null");

            exportableContent.Export();
        }

        void OnCommandExit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        void OnCommandAccountUnlock(object sender, EventArgs e)
        {
            using (var dialog = new AccountUnlockDialog()) {
                var result = dialog.ShowModal(this);
                if (result != null) {
                    Utilities.MoneroProcessManager.AccountManager.Passphrase = result;
                }
            }
        }

        void OnCommandAccountChangePassphrase(object sender, EventArgs e)
        {

        }

        void OnCommandShowWindowOptions(object sender, EventArgs e)
        {
            using (var dialog = new OptionsDialog()) {
                dialog.ShowModal(this);
            }
        }

        void OnCommandShowWindowAbout(object sender, EventArgs e)
        {
            using (var dialog = new AboutDialog()) {
                dialog.ShowModal(this);
            }
        }

        void OnDaemonRpcNetworkInformationChanged(object sender, NetworkInformationChangedEventArgs e)
        {
            var networkInformation = e.NetworkInformation;

            var connectionCount = networkInformation.ConnectionCountTotal;
            var blockTimeRemainingString = networkInformation.BlockTimeRemaining.ToStringReadable();

            var syncBarProgressPercentage = (double)networkInformation.BlockHeightDownloaded / networkInformation.BlockHeightTotal * 100;
            var syncBarText = string.Format(
                Utilities.InvariantCulture,
                Desktop.Properties.Resources.StatusBarSyncTextMain,
                networkInformation.BlockHeightRemaining,
                blockTimeRemainingString
            );
            var syncStatusIndicatorText = string.Format(
                Utilities.InvariantCulture,
                Desktop.Properties.Resources.StatusBarStatusTextMain,
                networkInformation.BlockHeightDownloaded,
                networkInformation.BlockHeightTotal,
                syncBarProgressPercentage.ToString("F2", CultureManager.CurrentCulture),
                blockTimeRemainingString
            );

            Application.Instance.AsyncInvoke(() => {
                var statusBarViewModel = StatusBarView.ViewModel;

                statusBarViewModel.ConnectionCount = connectionCount;
                statusBarViewModel.SyncStatusIndicatorText = syncStatusIndicatorText;

                if (statusBarViewModel.IsBlockchainSynced) return;
                statusBarViewModel.SyncStatusText = Desktop.Properties.Resources.StatusBarStatusSynchronizing;
                statusBarViewModel.SyncBarProgressValue = (int)Math.Floor(syncBarProgressPercentage * 100);
                statusBarViewModel.SyncBarText = syncBarText;
                statusBarViewModel.IsSyncBarVisible = true;
            });
        }

        void OnDaemonRpcBlockchainSynced(object sender, EventArgs e)
        {
            Application.Instance.AsyncInvoke(() => {
                // Enable sending coins, along with hiding the sync status
                StatusBarView.ViewModel.IsBlockchainSynced = true;
                SendCoinsView.ViewModel.IsBlockchainSynced = true;
            });
        }

        void OnAccountManagerProcessPassphraseRequested(object sender, PassphraseRequestedEventArgs e)
        {
            Application.Instance.AsyncInvoke(() => {
                if (e.IsFirstTime) {
                    // Let the user set the account's passphrase for the first time
                    using (var dialog = new AccountChangePassphraseDialog(false)) {
                        var result = dialog.ShowModal(this);
                        if (result != null) {
                            Utilities.MoneroProcessManager.AccountManager.Passphrase = result;
                        }
                    }

                } else {
                    // Request the account's passphrase in order to unlock it
                    OnCommandAccountUnlock(null, null);
                }
            });
        }

        void OnAccountManagerRpcInitialized(object sender, EventArgs e)
        {
            var transactions = Utilities.MoneroRpcManager.AccountManager.Transactions;
            Application.Instance.AsyncInvoke(() => {
                // Revoke the ability to (redundantly) unlock the account, and then allow changing the account's passphrase
                MenuSettings.Items.RemoveAt(0);
                MenuSettings.Items.Insert(0, CommandAccountChangePassphrase);

                for (var i = 0; i < transactions.Count; i++) {
                    Utilities.DataSourceAccountTransactions.Add(transactions[i]);
                }

                Utilities.BindingsToAccountTransactions.Update();
            });
        }

        void OnAccountManagerRpcAddressReceived(object sender, AccountAddressReceivedEventArgs e)
        {
            Application.Instance.AsyncInvoke(() => Utilities.BindingsToAccountAddress.Update());
        }

        static void OnAccountManagerRpcTransactionReceived(object sender, TransactionReceivedEventArgs e)
        {
            Application.Instance.AsyncInvoke(() => {
                Utilities.DataSourceAccountTransactions.Add(e.Transaction);
                Utilities.BindingsToAccountTransactions.Update();
            });
        }

        static void OnAccountManagerRpcTransactionChanged(object sender, TransactionChangedEventArgs e)
        {
            Application.Instance.AsyncInvoke(() => {
                var transactionIndex = e.TransactionIndex;
                Utilities.DataSourceAccountTransactions.RemoveAt(transactionIndex);
                Utilities.DataSourceAccountTransactions.Insert(transactionIndex, e.TransactionNewValue);
                Utilities.BindingsToAccountTransactions.Update();
            });
        }

        static void OnAccountManagerRpcBalanceChanged(object sender, AccountBalanceChangedEventArgs e)
        {
            Application.Instance.AsyncInvoke(() => {
                Utilities.BindingsToAccountBalance.Update();
                SendCoinsView.ViewModel.BalanceSpendable = e.AccountBalance.Spendable;
            });
        }

        public void UpdateLanguage()
        {
            UpdateBindings();
            RenderMenu();
            StatusBarView.UpdateLanguage();
        }
    }
}
