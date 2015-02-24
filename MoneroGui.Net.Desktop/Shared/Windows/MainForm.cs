using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroAPI;
using Jojatekok.MoneroGUI.Views.MainForm;
using System;
using System.Globalization;
using System.Threading;

namespace Jojatekok.MoneroGUI.Forms
{
	public sealed class MainForm : Form
	{
		public MainForm()
		{
            this.SetWindowProperties(
                () => MoneroGUI.Properties.Resources.TextClientName,
                new Size(850, 570),
                true // TODO: Remove this flag
		    );
		    this.SetLocationToCenterScreen();

            Closed += OnFormClosed;

		    Utilities.Initialize();
            Shown += delegate { InitializeCoreApi(); };

		    RenderMenu();
		    RenderContent();

		    var timer = new Timer(delegate {
                var culture = new CultureInfo("en");
                MoneroGUI.Properties.Resources.Culture = culture;
                Thread.CurrentThread.CurrentCulture = culture;

                Utilities.SyncContextMain.Post(
                    sender => {
                        UpdateBindings();
                        RenderMenu();
                    },
                    null
                );
		    }, null, 2000, 0);
		}

        private void OnFormClosed(object sender, EventArgs e)
        {
            if (Utilities.MoneroRpcManager != null) {
                Utilities.MoneroRpcManager.Dispose();
            }

            if (Utilities.MoneroProcessManager != null) {
                Utilities.MoneroProcessManager.DisposeSafely();
            }
        }

	    private void InitializeCoreApi()
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

	    private void RenderMenu()
	    {
	        var commandAccountBackupManager = new Command(OnCommandAccountBackupManager) {
                MenuText = MoneroGUI.Properties.Resources.MenuBackupManager,
                Image = Utilities.LoadImage("Save")
		    };

            var commandExport = new Command(OnCommandExport) {
                MenuText = MoneroGUI.Properties.Resources.MenuExport,
                Image = Utilities.LoadImage("Export"),
                Shortcut = Application.Instance.CommonModifier | Keys.E
            };

            var commandExit = new Command(OnCommandExit) {
                MenuText = MoneroGUI.Properties.Resources.MenuExit,
                Image = Utilities.LoadImage("Exit"),
                Shortcut = Application.Instance.CommonModifier | Keys.Q
            };

		    var commandAccountChangePassphrase = new Command(OnCommandAccountChangePassphrase) {
                Enabled = false,
                MenuText = MoneroGUI.Properties.Resources.MenuChangeAccountPassphrase,
                Image = Utilities.LoadImage("Key")
            };

            var commandShowWindowOptions = new Command(OnCommandShowWindowOptions) {
                MenuText = MoneroGUI.Properties.Resources.MenuOptions,
                Image = Utilities.LoadImage("Options"),
                Shortcut = Application.Instance.CommonModifier | Keys.O
            };

            var commandShowWindowDebug = new Command(OnCommandShowWindowDebug) {
                MenuText = MoneroGUI.Properties.Resources.MenuDebugWindow
            };

            var commandShowWindowAbout = new Command(OnCommandShowWindowAbout) {
                MenuText = MoneroGUI.Properties.Resources.MenuAbout,
                Image = Utilities.LoadImage("Information"),
            };

			Menu = new MenuBar {
				Items = {
					new ButtonMenuItem {
					    Text = MoneroGUI.Properties.Resources.MenuFile,
                        Items = {
                            commandAccountBackupManager,
                            commandExport,
                            new SeparatorMenuItem(),
                            commandExit
                        }
					},

                    new ButtonMenuItem {
					    Text = MoneroGUI.Properties.Resources.MenuSettings,
                        Items = {
                            commandAccountChangePassphrase,
                            new SeparatorMenuItem(),
                            commandShowWindowOptions
                        }
					},

                    new ButtonMenuItem {
					    Text = MoneroGUI.Properties.Resources.MenuHelp,
                        Items = {
                            commandShowWindowDebug,
                            new SeparatorMenuItem(),
                            commandShowWindowAbout
                        }
					}
				}
			};
	    }

	    private void RenderContent()
	    {
	        var tabPageOverview = new TabPage {
                Image = Utilities.LoadImage("Overview"),
                Content = new OverviewView()
            };
            tabPageOverview.SetTextBindingPath(() => " " + MoneroGUI.Properties.Resources.MainWindowOverview);

            var tabPageSendCoins = new TabPage {
                Image = Utilities.LoadImage("Send"),
                Content = new SendCoinsView()
            };
            tabPageSendCoins.SetTextBindingPath(() => " " + MoneroGUI.Properties.Resources.MainWindowSendCoins);

            var tabPageTransactions = new TabPage {
                Image = Utilities.LoadImage("Transaction"),
                Content = new TransactionsView()
            };
            tabPageTransactions.SetTextBindingPath(() => " " + MoneroGUI.Properties.Resources.MainWindowTransactions);

            var tabPageAddressBook = new TabPage {
                Image = Utilities.LoadImage("Contact"),
                Content = new AddressBookView()
            };
            tabPageAddressBook.SetTextBindingPath(() => " " + MoneroGUI.Properties.Resources.TextAddressBook);

            var tabControl = new TabControl();
            var tabControlPages = tabControl.Pages;
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
                            Content = tabControl
                        }
                    ) { ScaleHeight = true },

                    new TableRow(
                        new Panel {
                            BackgroundColor = Utilities.ColorStatusBar,
                            Padding = new Padding(Utilities.Padding4, Utilities.Padding2),
                            Content = new Label { Text = "Status bar" }
                        }
                    )
                }
            };
	    }

        private void OnCommandAccountBackupManager(object sender, EventArgs e)
	    {
	        
	    }

        private void OnCommandExport(object sender, EventArgs e)
        {

        }

        private void OnCommandExit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        private void OnCommandAccountChangePassphrase(object sender, EventArgs e)
        {

        }

        private void OnCommandShowWindowOptions(object sender, EventArgs e)
        {

        }

        private void OnCommandShowWindowDebug(object sender, EventArgs e)
        {

        }

        private void OnCommandShowWindowAbout(object sender, EventArgs e)
        {
            using (var dialog = new AboutDialog()) {
                dialog.ShowModal(this);
            }
        }

        private void OnDaemonProcessLogMessage(object sender, LogMessageReceivedEventArgs e)
        {

        }

        private void OnDaemonRpcNetworkInformationChanged(object sender, NetworkInformationChangedEventArgs e)
        {

        }

        private void OnDaemonRpcBlockchainSynced(object sender, EventArgs e)
        {

        }

        private void OnAccountManagerProcessLogMessage(object sender, LogMessageReceivedEventArgs e)
        {

        }

        private void OnAccountManagerProcessPassphraseRequested(object sender, PassphraseRequestedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => {
                if (e.IsFirstTime) {
                    // Let the user set the account's passphrase for the first time
                    // TODO

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

	    private void OnAccountManagerRpcInitialized(object sender, EventArgs e)
	    {
            var transactions = Utilities.MoneroRpcManager.AccountManager.Transactions;
            Utilities.SyncContextMain.Post(s => {
                for (var i = 0; i < transactions.Count; i++) {
                    Utilities.AccountTransactions.Add(transactions[i]);
                }

                Utilities.BindingsToAccountTransactions.Update();
            }, null);
	    }

        private void OnAccountManagerRpcAddressReceived(object sender, AccountAddressReceivedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => Utilities.BindingsToAccountAddress.Update(), null);
        }

        private void OnAccountManagerRpcTransactionReceived(object sender, TransactionReceivedEventArgs e)
        {
            Utilities.AccountTransactions.Add(e.Transaction);
            Utilities.SyncContextMain.Post(s => Utilities.BindingsToAccountTransactions.Update(), null);
        }

        private void OnAccountManagerRpcTransactionChanged(object sender, TransactionChangedEventArgs e)
        {
            Utilities.AccountTransactions[e.TransactionIndex] = e.TransactionNewValue;
            Utilities.SyncContextMain.Post(s => Utilities.BindingsToAccountTransactions.Update(), null);
        }

        private void OnAccountManagerRpcBalanceChanged(object sender, AccountBalanceChangedEventArgs e)
        {
            Utilities.SyncContextMain.Post(s => Utilities.BindingsToAccountBalance.Update(), null);
        }
	}
}
