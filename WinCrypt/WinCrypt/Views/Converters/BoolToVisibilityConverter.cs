namespace WinCrypt.Views.Converters
{
    // Required namespaces
    using System;
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// Converts a boolean value to a <see cref="System.Windows.Visibility"/>
    /// </summary>
    public class BoolToVisibilityConverter : BaseValueConverter<BoolToVisibilityConverter>
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
            if (value == null) return Visibility.Collapsed;

            // Check passed boolean value
            return ((bool)value) switch
            {
                // Return Visible if value is true
                true => Visibility.Visible,

                // Return Hidden if value is false
                false => Visibility.Hidden
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
