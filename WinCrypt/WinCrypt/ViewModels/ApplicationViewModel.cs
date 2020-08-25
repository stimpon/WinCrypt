namespace WinCrypt
{
    /// <summary>
    /// This is the programs underlaying ViewModel
    /// </summary>
    public class ApplicationViewModel
    {
        /// <summary>
        /// Tells the view wich page to show
        /// </summary>
        public ApplicationPages CurrentPage { get; set; }

        /// <summary>
        /// If the user opened a file with WinCrypt, then this
        /// will be the full file path
        /// </summary>
        public string OpenedFile { get; set; } = string.Empty;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationViewModel()
        {
            // Set the current page to the main page
            CurrentPage = ApplicationPages.EncryptPage;
        }
    }
}
