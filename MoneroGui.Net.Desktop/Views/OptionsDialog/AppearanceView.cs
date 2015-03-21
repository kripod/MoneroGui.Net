using Eto.Forms;
using System;

namespace Jojatekok.MoneroGUI.Views.OptionsDialog
{
    public class AppearanceView : TableLayout, IOptionsTabPageView
    {
        private readonly ComboBox _comboBoxLanguages = new ComboBox { ReadOnly = true };

        private ComboBox ComboBoxLanguages {
            get { return _comboBoxLanguages; }
        }

        public AppearanceView()
        {
            Spacing = Utilities.Spacing2;

            LoadSettings();

            Rows.Add(new TableRow(
                new Label { Text = MoneroGUI.Properties.Resources.OptionsAppearanceLanguage },
                ComboBoxLanguages
            ));

            Rows.Add(new TableRow());
        }

        void OnCultureManagerSupportedLanguagesLoaded(object sender, EventArgs e)
        {
            Utilities.SyncContextMain.Post(s => LoadLanguageValues(), null);
        }

        private void LoadLanguageValues()
        {
            var supportedLanguageStrings = CultureManager.SupportedLanguageStrings;

            // Check whether the list of languages has been loaded
            if (supportedLanguageStrings == null) return;
            CultureManager.SupportedLanguagesLoaded -= OnCultureManagerSupportedLanguagesLoaded;

            // Populate the ComboBox of available languages
            for (var i = 0; i < supportedLanguageStrings.Length; i++) {
                ComboBoxLanguages.Items.Add(supportedLanguageStrings[i]);
            }

            // In the ComboBox, select the language which is being used
            if (SettingsManager.Appearance.LanguageCode == Utilities.DefaultLanguageCode) {
                ComboBoxLanguages.SelectedIndex = 0;
                return;
            }

            var activeLanguageIndex = CultureManager.SupportedLanguageCultures.IndexOf(CultureManager.CurrentCulture);
            if (activeLanguageIndex >= 0) {
                ComboBoxLanguages.SelectedIndex = activeLanguageIndex;
            }
        }

        void LoadSettings()
        {
            CultureManager.SupportedLanguagesLoaded += OnCultureManagerSupportedLanguagesLoaded;
            LoadLanguageValues();
        }

        public void ApplySettings()
        {
            if (ComboBoxLanguages.Items != null && ComboBoxLanguages.SelectedIndex >= 0) {
                CultureManager.SetCulture(CultureManager.SupportedLanguageCultures[ComboBoxLanguages.SelectedIndex]);
            }
        }
    }
}
