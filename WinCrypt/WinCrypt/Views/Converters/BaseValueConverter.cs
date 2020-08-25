namespace WinCrypt
{
    /// <summary>
    /// Required namespaces 
    /// </summary>
    #region Namespaces
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;
    #endregion

    /// <summary>
    /// Base ValueConverter for all other ValueConverters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter where T: class, new()
    {
        /// <summary>
        /// Single instance of this converter
        /// </summary>
        private T Converter { get; set; }

        /// <summary>
        /// Gets implemented by the inheriting converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// Gets implemented by the inheriting converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        /// <summary>
        /// Provides the XAML with an instance of this converter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // Return the converter if an instance has been set, else return a new instance
            return Converter ?? new T();
        }
    }
}
