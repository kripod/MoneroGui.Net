using System.Globalization;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI
{
    public class ValidationRuleCoinAtomicValuePositive : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = Helper.GetBoundValue(value) as ulong?;
            if (input == null || input.Value == 0) {
                return new ValidationResult(false, null);
            }

            return new ValidationResult(true, null);
        }
    }
}
