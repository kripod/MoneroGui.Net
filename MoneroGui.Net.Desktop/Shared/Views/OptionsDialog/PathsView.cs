using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Controls;
using System.Collections.Generic;

namespace Jojatekok.MoneroGUI.Views.OptionsDialog
{
    public class PathsView : TableLayout, IOptionsTabPageView
    {
        private readonly PathSelector _pathSelectorDirectoryDaemonData = new PathSelector {
            Padding = new Padding(Utilities.Padding3, 0, 0, Utilities.Padding3)
        };
        private readonly PathSelector _pathSelectorFileAccountData = new PathSelector {
            Padding = new Padding(Utilities.Padding3, 0, 0, Utilities.Padding3),
            Filters = new HashSet<FileDialogFilter> {
                new FileDialogFilter(MoneroGUI.Properties.Resources.TextFilterAccountFiles, Utilities.FileFilterAccount)
            }
        };
        private readonly PathSelector _pathSelectorDirectoryAccountBackups = new PathSelector {
            Padding = new Padding(Utilities.Padding3, 0, 0, Utilities.Padding3)
        };

        private readonly PathSelector _pathSelectorSoftwareDaemon = new PathSelector {
            Padding = new Padding(Utilities.Padding3, 0, 0, Utilities.Padding3),
            Filters = new HashSet<FileDialogFilter> {
                new FileDialogFilter(MoneroGUI.Properties.Resources.TextFilterExecutableFiles, Utilities.FileFilterExecutable),
                new FileDialogFilter(MoneroGUI.Properties.Resources.TextFilterAllFiles, Utilities.FileFilterAll)
            }
        };
        private readonly PathSelector _pathSelectorSoftwareAccountManager = new PathSelector {
            Padding = new Padding(Utilities.Padding3, 0, 0, Utilities.Padding3),
            Filters = new HashSet<FileDialogFilter> {
                new FileDialogFilter(MoneroGUI.Properties.Resources.TextFilterExecutableFiles, Utilities.FileFilterExecutable),
                new FileDialogFilter(MoneroGUI.Properties.Resources.TextFilterAllFiles, Utilities.FileFilterAll)
            }
        };

        private PathSelector PathSelectorDirectoryDaemonData {
            get { return _pathSelectorDirectoryDaemonData; }
        }

        private PathSelector PathSelectorFileAccountData {
            get { return _pathSelectorFileAccountData; }
        }

        private PathSelector PathSelectorDirectoryAccountBackups {
            get { return _pathSelectorDirectoryAccountBackups; }
        }

        private PathSelector PathSelectorSoftwareDaemon {
            get { return _pathSelectorSoftwareDaemon; }
        }

        private PathSelector PathSelectorSoftwareAccountManager {
            get { return _pathSelectorSoftwareAccountManager; }
        }

        public PathsView()
        {
            LoadSettings();

            Rows.Add(new TableRow(
                new Panel {
                    Content = new Label { Text = MoneroGUI.Properties.Resources.OptionsPathsDirectoryDaemonData },
                    Padding = new Padding(0, 0, 0, Utilities.Padding3)
                },
                PathSelectorDirectoryDaemonData
            ));

            Rows.Add(new TableRow(
                new Panel {
                    Content = new Label { Text = MoneroGUI.Properties.Resources.OptionsPathsFileAccountData },
                    Padding = new Padding(0, 0, 0, Utilities.Padding3)
                },
                PathSelectorFileAccountData
            ));

            Rows.Add(new TableRow(
                new Panel {
                    Content = new Label { Text = MoneroGUI.Properties.Resources.OptionsPathsDirectoryAccountBackups },
                    Padding = new Padding(0, 0, 0, Utilities.Padding3)
                },
                PathSelectorDirectoryAccountBackups
            ));

            Rows.Add(new TableRow(
                new Panel {
                    Content = new Separator(SeparatorOrientation.Horizontal),
                    Padding = new Padding(0, Utilities.Padding1, 0, Utilities.Padding3 + Utilities.Padding1)
                },
                new Panel {
                    Content = new Separator(SeparatorOrientation.Horizontal),
                    Padding = new Padding(0, Utilities.Padding1, 0, Utilities.Padding3 + Utilities.Padding1)
                }
            ));

            Rows.Add(new TableRow(
                new Panel {
                    Content = new Label { Text = MoneroGUI.Properties.Resources.OptionsPathsSoftwareDaemon },
                    Padding = new Padding(0, 0, 0, Utilities.Padding3)
                },
                PathSelectorSoftwareDaemon
            ));

            Rows.Add(new TableRow(
                new Panel {
                    Content = new Label { Text = MoneroGUI.Properties.Resources.OptionsPathsSoftwareAccountManager },
                    Padding = new Padding(0, 0, 0, Utilities.Padding3)
                },
                PathSelectorSoftwareAccountManager
            ));

            Rows.Add(new TableRow());
        }

        void LoadSettings()
        {
            var pathSettings = SettingsManager.Paths;
            PathSelectorDirectoryDaemonData.SelectedPath = pathSettings.DirectoryDaemonData;
            PathSelectorFileAccountData.SelectedPath = pathSettings.FileAccountData;
            PathSelectorDirectoryAccountBackups.SelectedPath = pathSettings.DirectoryAccountBackups;
            PathSelectorSoftwareDaemon.SelectedPath = pathSettings.SoftwareDaemon;
            PathSelectorSoftwareAccountManager.SelectedPath = pathSettings.SoftwareAccountManager;
        }

        public void ApplySettings()
        {
            var pathSettings = SettingsManager.Paths;
            pathSettings.DirectoryDaemonData = PathSelectorDirectoryDaemonData.SelectedPath;
            pathSettings.FileAccountData = PathSelectorFileAccountData.SelectedPath;
            pathSettings.DirectoryAccountBackups = PathSelectorDirectoryAccountBackups.SelectedPath;
            pathSettings.SoftwareDaemon = PathSelectorSoftwareDaemon.SelectedPath;
            pathSettings.SoftwareAccountManager = PathSelectorSoftwareAccountManager.SelectedPath;
        }
    }
}
