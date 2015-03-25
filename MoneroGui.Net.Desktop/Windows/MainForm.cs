using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroGUI.Desktop.Views.MainForm;
using System;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class MainForm : Form
    {
        private Command CommandExport { get; set; }

        private TabControl TabControlMain { get; set; }

        public MainForm()
        {
            this.SetWindowProperties(
                () => MoneroGUI.Desktop.Properties.Resources.TextClientName,
                new Size(850, 570),
                true // TODO: Remove this flag
            );
            this.SetLocationToCenterScreen();

            Shown += delegate { InitializeCoreApi(); };
            Closed += OnFormClosed;

            Utilities.Initialize(this);

            RenderMenu();
            RenderContent();
        }

        static void OnFormClosed(object sender, EventArgs e)
        {
            if (Utilities.MoneroRpcManager != null) {
                Utilities.MoneroRpcManager.Dispose();
            }

            if (Utilities.MoneroProcessManager != null) {
                Utilities.MoneroProcessManager.DisposeSafely();
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
                daemonProcess.OnLogMessage += OnDaemonProcessLogMessage;
                daemonProcess.Start();
            } else {
                // Initialize the daemon RPC manager immediately
                daemonRpc.Initialize();
            }

            if (SettingsManager.Network.IsProcessAccountManagerHostedLocally) {
                // Initialize the account manager's RPC wrapper as soon as the corresponding process is available
                var accountManagerProcess = Utilities.MoneroProcessManager.AccountManager;
                accountManagerProcess.Initialized += delegate { accountManagerRpc.Initialize(); };
                accountManagerProcess.OnLogMessage += OnAccountManagerProcessLogMessage;
                accountManagerProcess.PassphraseRequested += OnAccountManagerProcessPassphraseRequested;
                accountManagerProcess.Start();
            } else {
                // Initialize the account manager's RPC wrapper immediately
                accountManagerRpc.Initialize();
            }
        }

        public void RenderMenu()
        {
            var commandAccountBackupManager = new Command(OnCommandAccountBackupManager) {
                MenuText = MoneroGUI.Desktop.Properties.Resources.MenuBackupManager,
                Image = Utilities.LoadImage("Save")
            };

            CommandExport = new Command(OnCommandExport) {
                MenuText = MoneroGUI.Desktop.Properties.Resources.MenuExport,
                Image = Utilities.LoadImage("Export"),
                Shortcut = Application.Instance.CommonModifier | Keys.E,
                Enabled = false
            };

            var commandExit = new Command(OnCommandExit) {
                MenuText = MoneroGUI.Desktop.Properties.Resources.MenuExit,
                Image = Utilities.LoadImage("Exit"),
                Shortcut = Application.Instance.CommonModifier | Keys.Q
            };

            var commandAccountChangePassphrase = new Command(OnCommandAccountChangePassphrase) {
                MenuText = MoneroGUI.Desktop.Properties.Resources.MenuChangeAccountPassphrase,
                Image = Utilities.LoadImage("Key"),
                Enabled = false
            };

            var commandShowWindowOptions = new Command(OnCommandShowWindowOptions) {
                MenuText = MoneroGUI.Desktop.Properties.Resources.MenuOptions,
                Image = Utilities.LoadImage("Options"),
                Shortcut = Application.Instance.CommonModifier | Keys.O
            };

            var commandShowWindowAbout = new Command(OnCommandShowWindowAbout) {
                MenuText = MoneroGUI.Desktop.Properties.Resources.MenuAbout,
                Image = Utilities.LoadImage("Information"),
            };

            Menu = new MenuBar {
                Items = {
                    new ButtonMenuItem {
                        Text = MoneroGUI.Desktop.Properties.Resources.MenuFile,
                        Items = {
                            commandAccountBackupManager,
                            CommandExport,
                            new SeparatorMenuItem(),
                            commandExit
                        }
                    },

                    new ButtonMenuItem {
                        Text = MoneroGUI.Desktop.Properties.Resources.MenuSettings,
                        Items = {
                            commandAccountChangePassphrase,
                            new SeparatorMenuItem(),
                            commandShowWindowOptions
                        }
                    },

                    new ButtonMenuItem {
                        Text = MoneroGUI.Desktop.Properties.Resources.MenuHelp,
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
            tabPageOverview.SetTextBindingPath(() => " " + MoneroGUI.Desktop.Properties.Resources.MainWindowOverview);

            var tabPageSendCoins = new TabPage {
                Image = Utilities.LoadImage("Send"),
                Content = new SendCoinsView()
            };
            tabPageSendCoins.SetTextBindingPath(() => " " + MoneroGUI.Desktop.Properties.Resources.MainWindowSendCoins);

            var tabPageTransactions = new TabPage {
                Image = Utilities.LoadImage("Transaction"),
                Content = new TransactionsView()
            };
            tabPageTransactions.SetTextBindingPath(() => " " + MoneroGUI.Desktop.Properties.Resources.MainWindowTransactions);

            var tabPageAddressBook = new TabPage {
                Image = Utilities.LoadImage("Contact"),
                Content = new AddressBookView()
            };
            tabPageAddressBook.SetTextBindingPath(() => " " + MoneroGUI.Desktop.Properties.Resources.TextAddressBook);

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

        void OnDaemonProcessLogMessage(object sender, LogMessageReceivedEventArgs e)
        {

        }

        void OnDaemonRpcNetworkInformationChanged(object sender, NetworkInformationChangedEventArgs e)
        {
            var networkInformation = e.NetworkInformation;

            var connectionCount = networkInformation.ConnectionCountTotal;
            var blockTimeRemainingString = networkInformation.BlockTimeRemaining.ToStringReadable();

            var syncBarProgressPercentage = (double)networkInformation.BlockHeightDownloaded / networkInformation.BlockHeightTotal * 100;
            var syncBarText = string.Format(
                Utilities.InvariantCulture,
                MoneroGUI.Desktop.Properties.Resources.StatusBarSyncTextMain,
                networkInformation.BlockHeightRemaining,
                blockTimeRemainingString
            );
            var syncStatusIndicatorText = string.Format(
                Utilities.InvariantCulture,
                MoneroGUI.Desktop.Properties.Resources.StatusBarStatusTextMain,
                networkInformation.BlockHeightDownloaded,
                networkInformation.BlockHeightTotal,
                syncBarProgressPercentage.ToString("F2", CultureManager.CurrentCulture),
                blockTimeRemainingString
            );

            Utilities.SyncContextMain.Post(s => {
                var statusBarViewModel = StatusBarView.ViewModel;

                statusBarViewModel.ConnectionCount = connectionCount;
                statusBarViewModel.SyncStatusIndicatorText = syncStatusIndicatorText;

                if (statusBarViewModel.IsBlockchainSynced) return;
                statusBarViewModel.SyncStatusText = MoneroGUI.Desktop.Properties.Resources.StatusBarStatusSynchronizing;
                statusBarViewModel.SyncBarProgressValue = (int)Math.Floor(syncBarProgressPercentage * 100);
                statusBarViewModel.SyncBarText = syncBarText;
                statusBarViewModel.IsSyncBarVisible = true;
            }, null);
        }

        void OnDaemonRpcBlockchainSynced(object sender, EventArgs e)
        {
            Utilities.SyncContextMain.Post(s => {
                // Enable sending coins, along with hiding the sync status
                StatusBarView.ViewModel.IsBlockchainSynced = true;
                //SendCoinsView.ViewModel.IsBlockchainSynced = true;
            }, null);
        }

        void OnAccountManagerProcessLogMessage(object sender, LogMessageReceivedEventArgs e)
        {

        }

        void OnAccountManagerProcessPassphraseRequested(object sender, PassphraseRequestedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => {
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
                    using (var dialog = new AccountUnlockDialog()) {
                        var result = dialog.ShowModal(this);
                        if (result != null) {
                            Utilities.MoneroProcessManager.AccountManager.Passphrase = result;
                        }
                    }
                }
            }, null);
        }

        void OnAccountManagerRpcInitialized(object sender, EventArgs e)
        {
            var transactions = Utilities.MoneroRpcManager.AccountManager.Transactions;
            Utilities.SyncContextMain.Post(s => {
                for (var i = 0; i < transactions.Count; i++) {
                    Utilities.DataSourceAccountTransactions.Add(transactions[i]);
                }

                Utilities.DataSourceAccountTransactions.Sort = (x, y) => y.Index.CompareTo(x.Index);
                Utilities.BindingsToAccountTransactions.Update();
            }, null);
        }

        void OnAccountManagerRpcAddressReceived(object sender, AccountAddressReceivedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => Utilities.BindingsToAccountAddress.Update(), null);
        }

        static void OnAccountManagerRpcTransactionReceived(object sender, TransactionReceivedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => {
                Utilities.DataSourceAccountTransactions.Add(e.Transaction);
                Utilities.BindingsToAccountTransactions.Update();
            }, null);
        }

        static void OnAccountManagerRpcTransactionChanged(object sender, TransactionChangedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => {
                var transactionIndex = e.TransactionIndex;
                Utilities.DataSourceAccountTransactions.RemoveAt(transactionIndex);
                Utilities.DataSourceAccountTransactions.Insert(transactionIndex, e.TransactionNewValue);
                Utilities.BindingsToAccountTransactions.Update();
            }, null);
        }

        static void OnAccountManagerRpcBalanceChanged(object sender, AccountBalanceChangedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => Utilities.BindingsToAccountBalance.Update(), null);
        }

        public void UpdateLanguage()
        {
            UpdateBindings();
            RenderMenu();
            StatusBarView.UpdateLanguage();
        }
    }
}
