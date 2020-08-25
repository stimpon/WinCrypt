namespace WinCrypt
{
    /// <summary>
    /// Required namespaces 
    /// </summary>
    #region Namespaces
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// Base ViewModel class for all other ViewModels to inherit from
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Is called when a property has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
    }
}
