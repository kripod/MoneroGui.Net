using Eto.Forms;
using Jojatekok.MoneroGUI.Windows;
using System;

namespace Jojatekok.MoneroGUI.Desktop
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Eto.Style.Add<Label>(null, label => {
                label.VerticalAlign = VerticalAlign.Middle;
            });

            new Application(Eto.Platform.Detect).Run(new MainForm());
        }
    }
}
