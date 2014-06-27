using System;

namespace Jojatekok.MoneroGUI.Views.OptionsWindow
{
    public partial class AppearanceView
    {
        public AppearanceView()
        {
            InitializeComponent();

#if DEBUG
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
#endif

            // <-- Load settings -->
            CultureManager.SupportedLanguagesLoaded += CultureManager_SupportedLanguagesLoaded;
            ShowLanguageValues();
        }

        public void ApplySettings()
        {
            if (ComboBoxLanguages.ItemsSource != null && ComboBoxLanguages.SelectedIndex >= 0) {
                var selectedCulture = CultureManager.SupportedLanguageCultures[ComboBoxLanguages.SelectedIndex];

                CultureManager.SetCulture(selectedCulture);
            }
        }

        private void ShowLanguageValues()
        {
            var supportedLanguageStrings = CultureManager.SupportedLanguageStrings;

            // Check whether the list of languages has been loaded
            if (supportedLanguageStrings == null) return;
            CultureManager.SupportedLanguagesLoaded -= CultureManager_SupportedLanguagesLoaded;
            ComboBoxLanguages.ItemsSource = supportedLanguageStrings;

            // Check for the usage of the default language
            if (SettingsManager.Appearance.LanguageCode == StaticObjects.DefaultLanguageCode) {
                ComboBoxLanguages.SelectedIndex = 0;
                return;
            }

            var activeLanguageIndex = CultureManager.SupportedLanguageCultures.IndexOf(CultureManager.CurrentCulture);
            if (activeLanguageIndex >= 0) {
                ComboBoxLanguages.SelectedIndex = activeLanguageIndex;
            }
        }

        private void CultureManager_SupportedLanguagesLoaded(object sender, EventArgs e)
        {
            Dispatcher.Invoke(ShowLanguageValues);
        }
    }
}
