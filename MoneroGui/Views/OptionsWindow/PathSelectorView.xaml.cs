using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Windows;

namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class PathSelectorView
    {
        public string SelectedPath {
            get { return TextBoxPath.Text; }
            set { TextBoxPath.Text = value; }
        }

        public string Filter { get; set; }
        public int FilterIndex { get; set; }

        public PathSelectorView()
        {
            InitializeComponent();
        }

        private void ButtonSelectPath_Click(object sender, RoutedEventArgs e)
        {
            if (Filter != null) {
                // Handle file selection
                var dialog = new OpenFileDialog { Filter = Filter, FilterIndex = FilterIndex };

                if (TextBoxPath.Text.Length != 0) {
                    var lastSlashIndex = TextBoxPath.Text.LastIndexOf('\\');
                    if (lastSlashIndex != -1) {
                        var directory = Path.GetFullPath(TextBoxPath.Text.Substring(0, lastSlashIndex));
                        if (Directory.Exists(directory)) dialog.InitialDirectory = directory;
                    }
                }

                if (dialog.ShowDialog() == true) TextBoxPath.Text = Helper.GetRelativePath(dialog.FileName);

            } else {
                // Handle folder selection
                var dialog = new VistaFolderBrowserDialog { RootFolder = Environment.SpecialFolder.MyComputer };

                if (TextBoxPath.Text.Length != 0) {
                    var directory = Path.GetFullPath(TextBoxPath.Text);
                    if (Directory.Exists(directory)) dialog.SelectedPath = directory;
                }

                if (dialog.ShowDialog() == true) {
                    var newPath = Helper.GetRelativePath(dialog.SelectedPath);
                    if (!newPath.EndsWith("\\", StringComparison.Ordinal)) newPath += "\\";
                    TextBoxPath.Text = newPath;
                }
            }
        }
    }
}
