using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinCrypt
{
    /// <summary>
    /// Interaction logic for DecryptPage.xaml
    /// </summary>
    public partial class DecryptPage : StaticBasePage<DecryptPageViewModel>, IHavePassword
    {
        public DecryptPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The decryption key
        /// </summary>
        public SecureString Password { get => PbPassword.SecurePassword; }
    }
}
