using Eto.Drawing;
using Eto.Forms;
using Jojatekok.MoneroGUI.Desktop.Controls;

namespace Jojatekok.MoneroGUI.Desktop.Views.MainForm
{
    public sealed class StatusBarView : TableLayout
    {
        private const int StatusBarHeight = 22;

        public static readonly StatusBarViewModel ViewModel = new StatusBarViewModel();

        private static readonly Label LabelSyncStatus = new Label();
        private static readonly Label LabelConnectionCount = new Label();

        private static readonly ImageView ImageViewSyncStatus = new ImageView();
        private static readonly ImageView ImageViewConnectionCount = new ImageView();

        private static readonly ProgressBarWithText ProgressBarSyncStatus = new ProgressBarWithText { MaxValue = 10000, Height = StatusBarHeight };

        public StatusBarView()
        {
            Spacing = Utilities.Spacing2;
            Height = StatusBarHeight;

            DataContext = ViewModel;

            LabelSyncStatus.BindDataContext<string>("Text", "SyncStatusText");
            LabelConnectionCount.BindDataContext<string>("Text", "ConnectionCountText");

            ImageViewSyncStatus.BindDataContext<string>("ToolTip", "SyncStatusIndicatorText");
            ImageViewSyncStatus.BindDataContext<Image>("Image", "SyncStatusIndicatorImage");
            ImageViewConnectionCount.BindDataContext<Image>("Image", "ConnectionCountIndicatorImage");

            ProgressBarSyncStatus.BindDataContext<bool>("Visible", "IsSyncBarVisible");
            ProgressBarSyncStatus.BindDataContext<int>("Value", "SyncBarProgressValue");
            ProgressBarSyncStatus.BindDataContext<string>("Text", "SyncBarText");

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
