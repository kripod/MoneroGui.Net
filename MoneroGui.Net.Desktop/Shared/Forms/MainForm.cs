using Eto.Drawing;
using Eto.Forms;
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
		    this.SetFormProperties(
                () => MoneroGUI.Properties.Resources.TextClientName,
                new Size(850, 570),
                true // TODO: Remove this flag
		    );
		    this.SetLocationToCenterScreen();

		    Utilities.Initialize();

		    RenderMenu();
		    RenderContent();

		    var timer = new Timer(delegate {
                var culture = new CultureInfo("en");
                MoneroGUI.Properties.Resources.Culture = culture;
                Thread.CurrentThread.CurrentCulture = culture;

                Utilities.SyncContextMain.Send(
                    sender => {
                        UpdateBindings();
                        RenderMenu();
                    },
                    null
                );
		    }, null, 2000, 0);
		}

	    private void RenderMenu()
	    {
	        var commandAccountBackupManager = new Command(OnCommandAccountBackupManager) {
                MenuText = MoneroGUI.Properties.Resources.MenuBackupManager,
                Image = Utilities.LoadImage("Save")
		    };

            var commandExport = new Command(OnExport) {
                MenuText = MoneroGUI.Properties.Resources.MenuExport,
                Image = Utilities.LoadImage("Export"),
                Shortcut = Application.Instance.CommonModifier | Keys.E
            };

            var commandExit = new Command(OnExit) {
                MenuText = MoneroGUI.Properties.Resources.MenuExit,
                Image = Utilities.LoadImage("Exit"),
                Shortcut = Application.Instance.CommonModifier | Keys.Q
            };

		    var commandAccountChangePassphrase = new Command(OnAccountChangePassphrase) {
                Enabled = false,
                MenuText = MoneroGUI.Properties.Resources.MenuChangeAccountPassphrase,
                Image = Utilities.LoadImage("Key")
            };

            var commandShowWindowOptions = new Command(OnShowWindowOptions) {
                MenuText = MoneroGUI.Properties.Resources.MenuOptions,
                Image = Utilities.LoadImage("Options"),
                Shortcut = Application.Instance.CommonModifier | Keys.O
            };

            var commandShowWindowDebug = new Command(OnShowWindowDebug) {
                MenuText = MoneroGUI.Properties.Resources.MenuDebugWindow
            };

            var commandShowWindowAbout = new Command(OnShowWindowAbout) {
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
                Image = Utilities.LoadImage("Transaction")
            };
            tabPageTransactions.SetTextBindingPath(() => " " + MoneroGUI.Properties.Resources.MainWindowTransactions);

            var tabPageAddressBook = new TabPage {
                Image = Utilities.LoadImage("Contact")
            };
            tabPageAddressBook.SetTextBindingPath(() => " " + MoneroGUI.Properties.Resources.TextAddressBook);

            var tabControl = new TabControl();
            var tabControlPages = tabControl.Pages;
            tabControlPages.Add(tabPageOverview);
            tabControlPages.Add(tabPageSendCoins);
            tabControlPages.Add(tabPageTransactions);
            tabControlPages.Add(tabPageAddressBook);

	        for (var i = tabControlPages.Count - 1; i >= 0; i--) {
	            tabControlPages[i].Padding = new Padding(Utilities.PaddingMedium);
	        }

            Content = new TableLayout {
                Rows = {
                    new TableRow(
                        new Panel {
                            Padding = new Padding(Utilities.PaddingLarge),
                            Content = tabControl
                        }
                    ) { ScaleHeight = true },

                    new TableRow(
                        new Panel {
                            BackgroundColor = Utilities.ColorStatusBar,
                            Padding = new Padding(Utilities.PaddingLarge, Utilities.PaddingSmall),
                            Content = new Label { Text = "Status bar" }
                        }
                    )
                }
            };
	    }

        private void OnCommandAccountBackupManager(object sender, EventArgs e)
	    {
	        
	    }

        private void OnExport(object sender, EventArgs e)
        {

        }

        private void OnExit(object sender, EventArgs e)
        {
            Application.Instance.Quit();
        }

        private void OnAccountChangePassphrase(object sender, EventArgs e)
        {

        }

        private void OnShowWindowOptions(object sender, EventArgs e)
        {

        }

        private void OnShowWindowDebug(object sender, EventArgs e)
        {

        }

        private void OnShowWindowAbout(object sender, EventArgs e)
        {

        }
	}
}
