namespace WinCrypt
{
    // Required namespaces
    using System.Windows;
    using System.Text;
    using System.Security.Cryptography;
    using System.IO;
    using System.Diagnostics;
    using System.Security.Cryptography.Pkcs;
    using System;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="Args">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        public MainWindow()
        {
            // Do the normal stuff
            InitializeComponent();
        }
    }
}
