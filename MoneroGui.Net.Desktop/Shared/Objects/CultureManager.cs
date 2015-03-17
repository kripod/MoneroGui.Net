using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

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

        public static void Initialize()
        {
            var languageCode = SettingsManager.Appearance.LanguageCode;
            var languageCulture = languageCode == Utilities.DefaultLanguageCode ? Utilities.DefaultUiCulture : new CultureInfo(languageCode);
            CurrentCulture = languageCulture;

            Task.Factory.StartNew(LoadCultures);
        }

        private static void LoadCultures()
        {
            var resourceManager = new ResourceManager(typeof(Properties.Resources));
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var supportedCultures = new List<CultureInfo>(cultures.Length - 1);

            for (var i = cultures.Length - 1; i >= 0; i--) {
                var culture = cultures[i];
                if (Equals(culture, Utilities.InvariantCulture)) continue;

                if (resourceManager.GetResourceSet(culture, true, false) != null) {
                    supportedCultures.Add(culture);
                }
            }

            supportedCultures = supportedCultures.OrderBy(x => x.ThreeLetterWindowsLanguageName).ToList();
            supportedCultures.Insert(0, Utilities.InvariantCulture);
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

                var stringFormat = nativeName != displayName ? "{0}	{1} ({2})" : "{0}	{1}";
                supportedStrings[i] = string.Format(
                    Utilities.InvariantCulture,
                    stringFormat,
                    culture.ThreeLetterWindowsLanguageName,
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
            if (!Equals(culture, Utilities.InvariantCulture)) {
                SettingsManager.Appearance.LanguageCode = culture.ToString();

            } else {
                SettingsManager.Appearance.LanguageCode = Utilities.DefaultLanguageCode;
                culture = Utilities.DefaultUiCulture;
            }

            // Apply changes only if necessary
            if (culture.ToString() == CurrentCulture.ToString()) return;
            CurrentCulture = culture;

            var mainForm = Utilities.MainForm;
            mainForm.UpdateBindings();
            mainForm.RenderMenu();
        }
    }
}
