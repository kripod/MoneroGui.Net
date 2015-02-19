using Eto.Forms;

namespace Jojatekok.MoneroGUI.Controls
{
    public sealed class Separator : Panel
    {
        public Separator(SeparatorOrientation orientation)
        {
            if (orientation == SeparatorOrientation.Horizontal) {
                Height = 1;
            } else {
                Width = 1;
            }

            BackgroundColor = Utilities.ColorSeparator;
        }
    }

    public enum SeparatorOrientation
    {
        Horizontal,
        Vertical
    }
}
