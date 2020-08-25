namespace WinCrypt
{
    // Required namespaces >>
    using System;
    using System.IO;
    using System.Text;
    using System.Windows.Input;
    using System.Security.Cryptography;
    using static CryptographyHelpers;
    using System.Threading.Tasks;

    /// <summary>
    /// ViewModel for the <see cref="EncryptFileAES(string, byte[], bool)"/>
    /// </summary>
    public class EncryptPageViewModel : BaseViewModel
    {
        #region Public properties

        #region File properties

        /// <summary>
        /// Name of the current file w/o extension
        /// </summary>
        public string CurrentFileName { get; set; }

        /// <value>
        /// The current file.
        /// </value>
        public string CurrentFilePath { get; set; }

        /// <summary>
        /// Name of the current file w/o extension
        /// </summary>
        public string OriginalFileName { get; set; }

        #endregion

        /// <summary>
        /// Tells the View if a file is being encrypted right now
        /// </summary>
        public bool Idling { get; set; } = true;

        /// <summary>
        /// Set to true if the original file should be kept after encryption
        /// </summary>
        public bool DeleteOriginalFile { get; set; } = false;

        /// <summary>
        /// Password for the file
        /// </summary>
        public string EncryptionPassword { get; set; }

        /// <summary>
        /// Tells the view how big the opened file is
        /// </summary>
        public long FileSize { get; set; } = 1;

        /// <summary>
        /// Tells the view how many bytes have been encrypted
        /// </summary>
        public long EncryptedBytes { get; set; } = 0;

        /// <summary>
        /// Information received from the cryptography handler
        /// </summary>
        public string EncryptionInformation { get; set; } = String.Empty;

        #endregion

        #region Commands

        /// <summary>
        /// Command for encrypting the file
        /// </summary>
        public ICommand EncryptCommand { get; set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public EncryptPageViewModel()
        {
            // Only do this if the program is running
            if (ProgramState.IsRunning)
            {
                // Initial function calls >>
                Init();    
            }
        }

        #region Private methods

        /// <summary>
        /// Handles the received cryptographic result
        /// </summary>
        /// <param name="dr"></param>
        private async void HandleEncryptResult(CryptographicResult dr)
        {
            // If there was an error
            if (!dr.OK)
            {

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
        /// Should be called in the constructor,
        /// Function will create all commands for the <see cref="EncryptPage"/>
        /// </summary>
        private void Init()
        {
            // Subscribe to important events
            EncryptionProgress += EncryptPageViewModel_EncryptionProgress;
            EncryptionInfo += EncryptPageViewModel_EncryptionInfo;

            // Create the encrypt command
            EncryptCommand = new RelayCommand(async () => 
            {
                // Turn on encryption mode
                Idling = false;

                // Get info about the file
                FileInfo fi = new FileInfo(OriginalFileName);
                // Set file length
                FileSize = fi.Length;

                // Create hasher for the password
                SHA256 Hasher = SHA256.Create();
                // Encrypt the file
                var result = await EncryptFileAES(OriginalFileName, @$"{CurrentFilePath}\{CurrentFileName}{fi.Extension}",
                    Hasher.ComputeHash(Encoding.Default.GetBytes(EncryptionPassword)), DeleteOriginalFile);

                // Handle the recieved result
                HandleEncryptResult(result);
            });
        }

        /// <summary>
        /// When information is received from <see cref="EncryptionInfo"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncryptPageViewModel_EncryptionInfo(object sender, string e)
        {
            // Display the received information
            this.EncryptionInformation = e;
        }

        /// <summary>
        /// When encryption progress is received from <see cref="EncryptionProgress"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncryptPageViewModel_EncryptionProgress(object sender, long e)
        {
            // Update the progress
            this.EncryptedBytes = e;
        }

        #endregion
    }
}
