using System;
using System.Reflection;
using System.Threading;
using Eto.Drawing;
using Eto.Forms;
using System.Collections.Generic;
using System.Text;

namespace Jojatekok.MoneroGUI
{
    struct Utilities
    {
        public static readonly Color ColorStatusBar = Color.FromRgb(15855085);

        public static readonly Assembly ApplicationAssembly = Assembly.GetExecutingAssembly();
        public static readonly AssemblyName ApplicationAssemblyName = ApplicationAssembly.GetName();
        public static readonly string ApplicationAssemblyNameName = ApplicationAssembly.GetName().Name;

        public static readonly Version ApplicationVersionComparable = ApplicationAssemblyName.Version;
        public const string ApplicationVersionExtra = null;
        public static readonly string ApplicationVersionString = ApplicationVersionComparable.ToString(3) + (ApplicationVersionExtra != null ? "-" + ApplicationVersionExtra : null);

        public static SynchronizationContext SyncContextMain { get; set; }

        public static readonly Properties.Resources ResourcesInstance = new Properties.Resources();

        private static readonly ImageConverter ImageConverter = new ImageConverter();

        public static Image LoadImage(string resourceName)
        {
            return ImageConverter.ConvertFrom(
                ImageConverter.ResourcePrefix +
                "Jojatekok.MoneroGUI." + resourceName + ".png," +
                ApplicationAssemblyNameName
            ) as Image;
        }
    }
}
