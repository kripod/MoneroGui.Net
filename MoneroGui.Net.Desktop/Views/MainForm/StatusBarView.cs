using Eto.Drawing;
using Eto.Forms;

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

            // TODO: Fix this ToolTip binding on Mac
            if (!Utilities.EnvironmentPlatform.IsMac) {
                ImageViewSyncStatus.BindDataContext<string>("ToolTip", "SyncStatusIndicatorText");
            }

            ImageViewSyncStatus.BindDataContext<Image>("Image", "SyncStatusIndicatorImage");
            ImageViewConnectionCount.BindDataContext<Image>("Image", "ConnectionCountIndicatorImage");

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
