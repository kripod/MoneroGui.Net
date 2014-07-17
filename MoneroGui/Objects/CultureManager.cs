using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Jojatekok.MoneroGUI
{
    static class CultureManager
    {
        public static event EventHandler SupportedLanguagesLoaded;

        public static List<CultureInfo> SupportedLanguageCultures { get; private set; }

        private static string[] _supportedLanguageStrings;
        public static string[] SupportedLanguageStrings {
            get {
                if (_supportedLanguageStrings == null) return null;

                _supportedLanguageStrings[0] = Properties.Resources.OptionsAppearanceLanguageNameDefault;
                return _supportedLanguageStrings;
            }

            private set { _supportedLanguageStrings = value; }
        }

        public static CultureInfo CurrentCulture {
            get { return Properties.Resources.Culture; }

            set {
                Properties.Resources.Culture = value;
                Thread.CurrentThread.CurrentCulture = value;
            }
        }

        private static ObjectDataProvider _resourceProvider;
        public static ObjectDataProvider ResourceProvider {
            get {
                if (_resourceProvider == null) {
                    _resourceProvider = (ObjectDataProvider)Application.Current.FindResource("Resources");
                }

                return _resourceProvider;
            }
        }

        private static readonly Properties.Resources ResourceInstance = new Properties.Resources();
        public static Properties.Resources GetResourceInstance()
        {
            return ResourceInstance;
        }

        static CultureManager()
        {
            Task.Factory.StartNew(LoadCultures);
        }

        private static void LoadCultures()
        {
            var resourceManager = new ResourceManager(typeof(Properties.Resources));
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var supportedCultures = new List<CultureInfo>(cultures.Length - 1);

            for (var i = cultures.Length - 1; i >= 0; i--) {
                var culture = cultures[i];
                if (Equals(culture, Helper.InvariantCulture)) continue;

                if (resourceManager.GetResourceSet(culture, true, false) != null) {
                    supportedCultures.Add(culture);
                }
            }

            supportedCultures = supportedCultures.OrderBy(x => x.TwoLetterISOLanguageName).ToList();
            supportedCultures.Insert(0, Helper.InvariantCulture);
            SupportedLanguageCultures = supportedCultures;

            var supportedStrings = new string[supportedCultures.Count];

            for (var i = supportedCultures.Count - 1; i > 0; i--) {
                var culture = supportedCultures[i];
                var nativeName = culture.NativeName.UppercaseFirst();
                var displayName = culture.EnglishName.UppercaseFirst();
                
                var bracketIndex = nativeName.IndexOf('(');
                if (bracketIndex >= 0) {
                    nativeName = nativeName.Substring(0, bracketIndex - 1);
                }

                bracketIndex = displayName.IndexOf('(');
                if (bracketIndex >= 0) {
                    displayName = displayName.Substring(0, bracketIndex - 1);
                }

                var stringFormat = nativeName != displayName ? "[{0}] {1} ({2})" : "[{0}] {1}";
                supportedStrings[i] = string.Format(
                    Helper.InvariantCulture,
                    stringFormat,
                    culture.TwoLetterISOLanguageName.ToUpper(Helper.InvariantCulture),
                    nativeName,
                    displayName
                );
            }

            SupportedLanguageStrings = supportedStrings;
            if (SupportedLanguagesLoaded != null) SupportedLanguagesLoaded(null, EventArgs.Empty);
        }

        public static void SetCulture(CultureInfo culture)
        {
            // Save settings
            if (!Equals(culture, Helper.InvariantCulture)) {
                SettingsManager.Appearance.LanguageCode = culture.TwoLetterISOLanguageName;

            } else {
                SettingsManager.Appearance.LanguageCode = StaticObjects.DefaultLanguageCode;
                culture = Helper.DefaultUiCulture;
            }

            // Apply changes only if necessary
            if (culture.TwoLetterISOLanguageName == CurrentCulture.TwoLetterISOLanguageName) return;
            CurrentCulture = culture;
            ResourceProvider.Refresh();

            // Manually update views which use converters
            StaticObjects.MainWindow.TransactionsView.DataGridTransactions.Items.Refresh();
        }
    }
}
