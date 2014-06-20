using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class AppearanceView
    {
        private static List<CultureInfo> LanguageCultures { get; set; }
        private static string[] LanguageStrings { get; set; }

        public AppearanceView()
        {
            InitializeComponent();

#if DEBUG
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
#endif

            // <-- Load settings -->

            if (LanguageStrings == null) {
                // Load the list of available languages asynchronously
                Task.Factory.StartNew(LoadLanguages);

            } else {
                ShowLanguageValues();
            }
        }

        public void ApplySettings()
        {
            // TODO: Apply language settings at runtime if possible
            var appearanceSettings = SettingsManager.Appearance;

            if (ComboBoxLanguages.ItemsSource != null && ComboBoxLanguages.SelectedIndex >= 0) {
                var selectedCulture = LanguageCultures[ComboBoxLanguages.SelectedIndex];
                if (!Equals(selectedCulture, Helper.InvariantCulture)) {
                    appearanceSettings.LanguageCode = selectedCulture.TwoLetterISOLanguageName;
                } else {
                    appearanceSettings.LanguageCode = Helper.DefaultLanguageCode;
                    selectedCulture = null;
                }

                CultureInfo.DefaultThreadCurrentCulture = selectedCulture;
                CultureInfo.DefaultThreadCurrentUICulture = selectedCulture;
            }
        }

        private void LoadLanguages()
        {
            var resourceManager = new ResourceManager(typeof(Properties.Resources));
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var languageList = new List<CultureInfo>(cultures.Length - 1);

            for (var i = cultures.Length - 1; i >= 0; i--) {
                var culture = cultures[i];
                if (Equals(culture, Helper.InvariantCulture)) continue;

                if (resourceManager.GetResourceSet(culture, true, false) != null) {
                    languageList.Add(culture);
                }
            }

            languageList = languageList.OrderBy(x => x.TwoLetterISOLanguageName).ToList();
            languageList.Insert(0, Helper.InvariantCulture);
            LanguageCultures = languageList;
            LanguageStrings = new string[LanguageCultures.Count];
            LanguageStrings[0] = Properties.Resources.OptionsDisplayLanguageNameDefault;

            for (var i = LanguageCultures.Count - 1; i > 0; i--) {
                var culture = LanguageCultures[i];
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
                LanguageStrings[i] = string.Format(
                    Helper.InvariantCulture,
                    stringFormat,
                    culture.TwoLetterISOLanguageName.ToUpper(Helper.InvariantCulture),
                    nativeName,
                    displayName
                );
            }

            Dispatcher.Invoke(ShowLanguageValues);
        }

        private void ShowLanguageValues()
        {
            ComboBoxLanguages.ItemsSource = LanguageStrings;

            // Check for the usage of the default language
            if (SettingsManager.Appearance.LanguageCode == Helper.DefaultLanguageCode) {
                ComboBoxLanguages.SelectedIndex = 0;
                return;
            }

            var activeLanguageIndex = LanguageCultures.IndexOf(CultureInfo.CurrentCulture);
            if (activeLanguageIndex >= 0) {
                ComboBoxLanguages.SelectedIndex = activeLanguageIndex;
            }
        }
    }
}
