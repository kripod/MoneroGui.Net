using Eto;
using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public sealed class StatusBarView : TableLayout
    {
        public static readonly StatusBarViewModel ViewModel = new StatusBarViewModel();

        private static readonly Label LabelSyncStatus = new Label();
        private static readonly Label LabelConnectionCount = new Label();

        private static readonly ImageView ImageViewSyncStatus = new ImageView();
        private static readonly ImageView ImageViewConnectionCount = new ImageView();

        private static readonly ProgressBar ProgressBarSyncStatus = new ProgressBar();

        public StatusBarView()
        {
            Spacing = Utilities.Spacing2;

            DataContext = ViewModel;

            LabelSyncStatus.BindDataContext<string>("Text", "SyncStatusText");
            LabelConnectionCount.BindDataContext<string>("Text", "ConnectionCountText");

            // TODO: Make image bindings work on Linux
            //ImageViewSyncStatus.BindDataContext<Image>("Image", "SyncStatusIndicatorImage");
            ImageViewSyncStatus.BindDataContext<string>("ToolTip", "SyncStatusIndicatorText");
            //ImageViewConnectionCount.BindDataContext<Image>("Image", "ConnectionCountIndicatorImage");
            ImageViewSyncStatus.Image = ViewModel.SyncStatusIndicatorImage;
            ImageViewConnectionCount.Image = ViewModel.ConnectionCountIndicatorImage;

            ProgressBarSyncStatus.MaxValue = 10000;
            ProgressBarSyncStatus.BindDataContext<bool>("Visible", "IsSyncBarVisible");
            ProgressBarSyncStatus.BindDataContext<int>("Value", "SyncBarProgressValue");

            Rows.Add(
                new TableRow(
                    ImageViewSyncStatus,
                    LabelSyncStatus,
                    new TableCell(new Panel { Content = ProgressBarSyncStatus, Padding = new Padding(Utilities.Padding2, 0) }, true),
                    LabelConnectionCount,
                    ImageViewConnectionCount
                )
            );
        }

        public static void UpdateLanguage()
        {
            ViewModel.IsBlockchainSynced = ViewModel.IsBlockchainSynced;
            ViewModel.ConnectionCount = ViewModel.ConnectionCount;
        }
    }
}
