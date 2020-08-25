namespace WinCrypt
{
    // Required namespaces >>
    using System.IO;
    using System.Security.Cryptography;
    using System.Windows.Input;
    using System.Text;
    using System;
    using static CryptographyHelpers;
    using System.Threading.Tasks;

    /// <summary>
    /// ViewModel for the <see cref="MainPage"/>
    /// </summary>
    public class DecryptPageViewModel : BaseViewModel
    {
        #region Public properties

        /// <summary>
        /// The current file opened
        /// </summary>
        public string CurrentFile { get; set; }

        /// <summary>
        /// Set to true if the original file should be kept after encryption
        /// </summary>
        public bool DeleteOriginalFile { get; set; } = true;

        /// <summary>
        /// Password for the file
        /// </summary>
        public string DecryptionPassword { get; set; }

        /// <summary>
        /// Tells the view how big the opened file is
        /// </summary>
        public long FileSize { get; set; } = 1;

        /// <summary>
        /// Tells the view how many bytes have been encrypted
        /// </summary>
        public long DecryptedBytes { get; set; } = 0;

        /// <summary>
        /// Error message to show the user
        /// </summary>
        public string DecryptionInformation { get; set; } = String.Empty;

        /// <summary>
        /// Tells the view if a file is being decrypted right now
        /// </summary>
        public bool Idling { get; set; } = true;

        #endregion

        #region Commands

        /// <summary>
        /// Command for encrypting the file
        /// </summary>
        public ICommand DecryptCommand { get; set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public DecryptPageViewModel()
        {
            if (ProgramState.IsRunning)
            {
                // Initial function calls >>
                Init();
            }
        }

        #region Private methods

        /// <summary>
        /// Should be called in the constructor,
        /// Function will create all commands for the <see cref="EncryptPage"/>
        /// </summary>
        private void Init()
        {
            // Subscribe to important events
            DecryptionProgress += DecryptPageViewModel_DecryptionProgress;
            DecryptionInfo += DecryptPageViewModel_DecryptionInfo;

            // Create the encrypt command
            DecryptCommand = new RelayCommand(async () =>
            {
                // Turn on decryption mode
                Idling = false;

                // Set the file size
                FileSize = new FileInfo(CurrentFile).Length;

                // Create hasher for the password
                SHA256 Hasher = SHA256.Create();
                // Encrypt the file
                var res = await DecryptFileAES(CurrentFile,
                            Hasher.ComputeHash(Encoding.Default.GetBytes(DecryptionPassword)));

                // Handle the recieved result
                HandleDecryptResult(res);
            });
        }

        /// <summary>
        /// Handles the received cryptographic result
        /// </summary>
        /// <param name="dr"></param>
        private async void HandleDecryptResult(CryptographicResult dr)
        {
            // If there was an error
            if (!dr.OK)
            {
                // Check error message
                DecryptionInformation = (dr.Error) switch
                {
                    // Print out message depending on what the error is

                    ErrorTypes.WrongDecryptionKey => "Invalid decryption key or corrupted file",
                    ErrorTypes.FileNotFound => "Could not find file: " + CurrentFile,
                    ErrorTypes.DirectoryNotFound => "Directory does not exist: " + CurrentFile,
                    _ => String.Empty
                };
                // Turn off decryption mode
                Idling = true;
            }
            // Exit if there was no problems
            else 
            {
                // Wait 1 second before exiting
                await Task.Run(() => Task.Delay(1000));
                // Exit the application
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// When decryption information is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecryptPageViewModel_DecryptionInfo(object sender, string e)
        {
            // Display the received information string
            this.DecryptionInformation = e;
        }

        /// <summary>
        /// When encryption progress is received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecryptPageViewModel_DecryptionProgress(object sender, long e)
        {
            // Update the progress
            this.DecryptedBytes = e;
        }

        #endregion
    }
}