namespace WinCrypt
{
    // Required namespaces
    using System.Security;

    /// <summary>
    /// Interaction logic for EncryptFile.xaml
    /// </summary>
    public partial class EncryptPage : StaticBasePage<EncryptPageViewModel>, IHavePassword
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EncryptPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the secure password from the encryption key box
        /// </summary>
        public SecureString Password { get => PbPassword.SecurePassword; }
    }
}
