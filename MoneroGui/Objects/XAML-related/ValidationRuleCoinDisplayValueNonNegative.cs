using System.Globalization;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI
{
    public class ValidationRuleCoinDisplayValueNonNegative : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as double?;
            if (input != null && input.Value <= -0.0000000000005) {
                return new ValidationResult(false, null);
            }

            return new ValidationResult(true, null);
        }
    }
}
