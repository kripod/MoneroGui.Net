using Eto.Forms;
using System;

namespace Jojatekok.MoneroGUI.Desktop.Controls
{
    public sealed class ProgressBarWithText : PixelLayout
    {
        private readonly ProgressBar _progressBar = new ProgressBar();
        private readonly Label _label = new Label();

        private TableLayout TableLayoutProgressBar { get; set; }
        private ProgressBar ProgressBar { get { return _progressBar; } }
        private Label Label { get { return _label; } }

        public int Value {
            get { return ProgressBar.Value; }
            set { ProgressBar.Value = value; }
        }

        public int MaxValue {
            get { return ProgressBar.MaxValue; }
            set { ProgressBar.MaxValue = value; }
        }

        public string Text {
            get { return Label.Text; }
            set { Label.Text = value; }
        }

        public ProgressBarWithText()
        {
            TableLayoutProgressBar = new TableLayout(new TableRow(new TableCell(ProgressBar, true) { ScaleWidth = true }));

            Add(TableLayoutProgressBar, 0, 0);
            Add(Label, 0, 0);

            SizeChanged += OnControlSizeChanged;
        }

        void OnControlSizeChanged(object sender, EventArgs e)
        {
            var labelLocationX = (Width - Label.Width) / 2;
            var labelLocationY = (Height - Label.Height) / 2 + 1;

            Remove(Label);
            Remove(TableLayoutProgressBar);

            TableLayoutProgressBar.Width = Width;
            TableLayoutProgressBar.Height = Height;

            Add(TableLayoutProgressBar, 0, 0);
            Add(Label, labelLocationX, labelLocationY);
        }
    }
}
