namespace WinCrypt
{
    // Required namespaces
    using System.Windows.Controls;
    
    /// <summary>
    /// Standard BasePage that does'nt set the DataContext of the page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Windows.Controls.Page" />
    public class BasePage<T> : Page where T: BaseViewModel, new()
    {
        #region Public proprties

        /// <summary>
        /// The ViewModel for this page
        /// </summary>
        public T ViewModel
        {
            // Retirn the ViewModel
            get => viewModel;
            set
            {
                // Check if the ViewModel has changed
                if (viewModel == value) 
                    return;
                // Change the ViewModel if it has changed
                viewModel = value;
                // Update this page's datacontext
                this.DataContext = value;
            }
        }

        #endregion

        #region Private members

        /// <summary>
        /// Private instance of this page's ViewModel
        /// </summary>
        private T viewModel;

        #endregion
    }

    public class StandardBasePage<T> : BasePage<T> where T : BaseViewModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandardBasePage{T}"/> class.
        /// </summary>
        public StandardBasePage()
        {
            // Set the datacontext to a new instance of the passed ViewModel
            this.DataContext = new T();
        }
    }

    /// <summary>
    /// Static basePage that tries to find a constant instance of <see cref="T"/>
    /// in the <see cref="Container.ApplicationKernel"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="WinCrypt.BasePage{T}" />
    public class StaticBasePage<T> : BasePage<T> where T : BaseViewModel, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticBasePage{T}"/> class.
        /// </summary>
        public StaticBasePage()
        {
            // Get the dataxontext from the application kernel
            try { this.DataContext = Container.Get<T>(); }

            // If the ViewModel does not exist in the kernel...
            catch
            {
                // Set the datacontext to a new instance of it self
                this.DataContext = new T();
            }
        }
    }
}
