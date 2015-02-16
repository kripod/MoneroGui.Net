using Eto.Forms;
using Jojatekok.MoneroGUI.Forms;
using System;

namespace Jojatekok.MoneroGUI.Desktop
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platform.Detect).Run(new MainForm());
        }
    }
}
