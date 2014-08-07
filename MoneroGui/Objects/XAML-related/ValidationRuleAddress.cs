using System.Globalization;
using System.Windows.Controls;

namespace Jojatekok.MoneroGUI
{
    public class ValidationRuleAddress : ValidationRule
    {
        private const byte ValueLength = 95;
        private const char ValueCharFirst = '4';
        private const char ValueCharSecondRangeEnd = 'B';
        private const char ValueCharAnyRangeStart = '0';
        private const char ValueCharAnyRangeEnd = 'z';

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = Helper.GetBoundValue(value) as string;
            if (
                string.IsNullOrEmpty(input) ||
                input.Length != ValueLength ||
                input[0] != ValueCharFirst ||
                input[1] < ValueCharAnyRangeStart ||
                input[1] > ValueCharSecondRangeEnd
            ) {
                return new ValidationResult(false, null);
            }

            for (var i = 2; i < input.Length; i++) {
                var currentChar = input[i];
                if (currentChar < ValueCharAnyRangeStart || currentChar > ValueCharAnyRangeEnd) {
                    return new ValidationResult(false, null);
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
