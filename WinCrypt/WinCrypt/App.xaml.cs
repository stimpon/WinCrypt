namespace WinCrypt
{
    // Required namespaces
    using System;
    using System.IO;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // The program is now running
            ProgramState.IsRunning = true;

            // Do the standard stuff
            base.OnStartup(e);

            // Run the registry function
            RegistryHelpers.RegistrySetup();

            // Setup the IoC Container
            Container.SetupIoC();

            // If program was opened without any parameters
            if (e.Args.Length < 1) Environment.Exit(0); // Exit the program
            // Else...
            else
            {
                // 2 parameters should have been passed, the [0]file and [1]mode
                if (e.Args.Length != 2) throw new Exception("Something went wrong");

                // There is no need to bind both ViewModels if WinCrypt was opened with a file so we check 
                // if the file should be encrypted or decrypted and bind the one ViewModel
                switch (e.Args[1])
                {
                    // This mode means the user choose to encrypt a file
                    case "0":
                        // Set current page
                        Container.ApplicationViewModel.CurrentPage = ApplicationPages.EncryptPage;
                        // Bind a new constant instance of the EncryptPageViewModel to the application kernel
                        Container.ApplicationKernel.Bind<EncryptPageViewModel>().ToConstant(new EncryptPageViewModel());

                        // Set file properties
                        Container.Get<EncryptPageViewModel>().CurrentFilePath  = Path.GetDirectoryName(e.Args[0]);
                        Container.Get<EncryptPageViewModel>().CurrentFileName  = Path.GetFileNameWithoutExtension(e.Args[0]);
                        Container.Get<EncryptPageViewModel>().OriginalFileName = e.Args[0];
                        break;
                    // This mode means the user choose to decrypt a file
                    case "1":
                        // Set current page
                        Container.ApplicationViewModel.CurrentPage = ApplicationPages.DecryptPage;
                        // Bind a new constant instance of the DecryptPageViewModel to the application kernel
                        Container.ApplicationKernel.Bind<DecryptPageViewModel>().ToConstant(new DecryptPageViewModel());
                        
                        // Set file properties
                        Container.Get<DecryptPageViewModel>().CurrentFilePath  = Path.GetDirectoryName(e.Args[0]);
                        Container.Get<DecryptPageViewModel>().CurrentFileName  = Path.GetFileNameWithoutExtension(e.Args[0]);
                        Container.Get<DecryptPageViewModel>().OriginalFileName = e.Args[0];
                        break;

                    // Invalid mode was passed
                    default: return;
                }

            }

            // Start the main window and pass the startup arguments with it
            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
        }
    }
}
