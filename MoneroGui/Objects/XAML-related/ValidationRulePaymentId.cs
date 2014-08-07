using System.Globalization;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI
{
    public class ValidationRulePaymentId : ValidationRule
    {
        private const byte ValueMaxLength = 64;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = Helper.GetBoundValue(value) as string;
            if (string.IsNullOrEmpty(input)) return new ValidationResult(true, null);
            if (input.Length > ValueMaxLength) return new ValidationResult(false, null);

            for (var i = input.Length - 1; i >= 0; i--) {
                var currentChar = input[i];
                if (currentChar < '0' || char.ToUpper(currentChar, Helper.InvariantCulture) > 'F') {
                    return new ValidationResult(false, null);
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
