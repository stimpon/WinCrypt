namespace WinCrypt
{
    // Required namespaces
    using System;
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// Converts a boolean value to a <see cref="System.Windows.Visibility"/>
    /// </summary>
    public class BoolToOperationConverter : BaseValueConverter<BoolToOperationConverter>
    {
        /// <summary>
        /// Converts a boolean value and returns a <see cref="System.Windows.Visibility"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if converter received a null variable
            if (value == null) return String.Empty;

            // Check passed boolean value
            return ((bool)value) switch
            {
                // Return a new string that is "Cancel"
                true  => new String("Cancel"),

                // Check if this conversion request comes from the decryption or encryption page
                false => (parameter as String == "0") ? new String("Encrypt") : new String("Decrypt")
            };
        }

        /// <summary>
        /// Converts a <see cref="System.Windows.Visibility"/> to a boolean value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Does not need implementation for now...
            throw new NotImplementedException();
        }
    }
}
