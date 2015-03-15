using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Views.OptionsDialog;
using System.Diagnostics;

namespace Jojatekok.MoneroGUI.Windows
{
    public sealed class OptionsDialog : Dialog
    {
        public OptionsDialog()
        {
            this.SetWindowProperties(
                MoneroGUI.Properties.Resources.AccountUnlockWindowTitle,
                new Size(580, 420)
            );

            RenderContent();
        }

        void RenderContent()
        {
            Padding = new Padding(Utilities.Padding4);

            var tabControl = new TabControl();
            var tabControlPages = tabControl.Pages;
            tabControlPages.Add(new TabPage {
                Text = MoneroGUI.Properties.Resources.OptionsGeneral,
                Content = new GeneralView()
            });

            for (var i = tabControlPages.Count - 1; i >= 0; i--) {
                tabControlPages[i].Padding = new Padding(Utilities.Padding3);
            }

            var buttonOk = new Button { Text = MoneroGUI.Properties.Resources.TextOk };
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

            var buttonCancel = new Button { Text = MoneroGUI.Properties.Resources.TextCancel };
            AbortButton = buttonCancel;

            Content = new TableLayout(
                new TableRow(new TableCell(tabControl)) { ScaleHeight = true },

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
    }
}
