using System.Globalization;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI
{
    public class ValidationRuleAddress : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(!string.IsNullOrEmpty(Helper.GetBoundValue(value) as string), null);
        }
    }
}
