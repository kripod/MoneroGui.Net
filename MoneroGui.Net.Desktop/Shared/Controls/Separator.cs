using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace Jojatekok.MoneroGUI.Controls
{
    public sealed class Separator : Panel
    {
        public Separator(SeparatorOrientation orientation)
        {
            if (orientation == SeparatorOrientation.Horizontal) {
                Height = Utilities.PaddingMedium * 2 + 1;
                Padding = new Padding(0, Utilities.PaddingMedium);
            } else {
                Width = 1;
            }

            Content = new Panel { BackgroundColor = Utilities.ColorSeparator };
        }
    }

    public enum SeparatorOrientation
    {
        Horizontal,
        Vertical
    }
}
