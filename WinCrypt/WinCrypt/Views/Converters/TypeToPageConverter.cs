namespace WinCrypt
{
    /// <summary>
    /// Required namespaces 
    /// </summary>
    #region Namespaces
    using System;
    using System.Globalization;
    #endregion

    /// <summary>
    /// Converter to convert a <see cref="ApplicationPages"/> to a <see cref="BasePage{T}"/>
    /// </summary>
    public class TypeToPageConverter : BaseValueConverter<TypeToPageConverter>
    {
        /// <summary>
        /// Convert the Type to the corresponding page
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check that a value was passed
            if (value == null) return null;

            // Check what page to return
            return ((ApplicationPages)value) switch
            {
                // Return a new instance of a EncryptionPage
                ApplicationPages.EncryptPage => new EncryptPage(),

                // Return a new instance of a DecryptionPage
                ApplicationPages.DecryptPage => new DecryptPage(),

                // Invalid type was passed
                _ => null,
            };
        }

        /// <summary>
        /// Convert the page back into the correspinging type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Does not need implementation for now
            return null;
        }
    }
}
