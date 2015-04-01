using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Views.OptionsDialog;
using System;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Desktop.Windows
{
    public sealed class OptionsDialog : Dialog
    {
        private TabControl TabControlMain { get; set; }

        public OptionsDialog()
        {
            this.SetWindowProperties(
                Desktop.Properties.Resources.AccountUnlockWindowTitle,
                new Size(600, 0)
            );

            RenderContent();
        }

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            TabControlMain = new TabControl();
            var tabControlPages = TabControlMain.Pages;
            tabControlPages.Add(new TabPage {
                Text = Desktop.Properties.Resources.OptionsGeneral,
                Content = new GeneralView()
            });
            tabControlPages.Add(new TabPage {
                Text = Desktop.Properties.Resources.OptionsPaths,
                Content = new PathsView()
            });
            tabControlPages.Add(new TabPage {
                Text = Desktop.Properties.Resources.OptionsNetwork,
                Content = new NetworkView()
            });
            tabControlPages.Add(new TabPage {
                Text = Desktop.Properties.Resources.OptionsAppearance,
                Content = new AppearanceView()
            });

            // Make the window automatically sized
            TabControlMain.SelectedIndex = 2;
            SizeChanged += OnDialogSizeChanged;

            for (var i = tabControlPages.Count - 1; i >= 0; i--) {
                tabControlPages[i].Padding = new Padding(Utilities.Padding3);
            }

            var buttonOk = new Button { Text = Desktop.Properties.Resources.TextOk };
            buttonOk.Click += delegate {
                SettingsManager.IsAutoSaveEnabled = false;

                for (var i = tabControlPages.Count - 1; i >= 0; i--) {
                    var optionsTabPageView = tabControlPages[i].Content as IOptionsTabPageView;
                    Debug.Assert(optionsTabPageView != null, "optionsTabPageView != null");
                    optionsTabPageView.ApplySettings();
                }

                SettingsManager.IsAutoSaveEnabled = true;
                SettingsManager.SaveSettings();
                Close();
            };
            DefaultButton = buttonOk;

            var buttonCancel = new Button { Text = Desktop.Properties.Resources.TextCancel };
            AbortButton = buttonCancel;
            AbortButton.Click += delegate { Close(); };

            Content = new TableLayout(
                new TableRow(new TableCell(TabControlMain)) { ScaleHeight = true },

                new TableRow(
                    new TableLayout(
                        new TableRow(
                            new TableCell { ScaleWidth = true },
                            buttonOk,
                            buttonCancel
                        )
                    ) { Spacing = Utilities.Spacing3 }
                )
            ) { Spacing = Utilities.Spacing3 };
        }

        void OnDialogSizeChanged(object sender, EventArgs e)
        {
            MinimumSize = Size;
            TabControlMain.SelectedIndex = 0;

            SizeChanged -= OnDialogSizeChanged;
        }
    }
}
