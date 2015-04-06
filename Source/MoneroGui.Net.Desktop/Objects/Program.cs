using Eto;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Windows;
using System;

namespace Jojatekok.MoneroGUI.Desktop
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            AddStyles();

            new Application(Platform.Detect).Run(new MainForm());
        }

        static void AddStyles()
        {
            Style.Add<Label>(null, label => {
                label.VerticalAlignment = VerticalAlignment.Center;
            });
        }
    }
}
