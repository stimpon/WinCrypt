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
    using System.Threading;

    /// <summary>
    /// ViewModel for the <see cref="MainPage"/>
    /// </summary>
    public class DecryptPageViewModel : BaseViewModel
    {
        #region Public properties

        /// <summary>
        /// The current file opened
        /// </summary>
        public string CurrentFileName { get; set; }

        /// <summary>
        /// Gets or sets the current file path.
        /// </summary>
        public string CurrentFilePath { get; set; }

        /// <summary>
        /// Gets or sets the name of the original file.
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// Set to true if the original file should be kept after encryption
        /// </summary>
        public bool DeleteOriginalFile { get; set; } = true;

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
        /// Is the program encrypting a file right now?
        /// </summary>
        public bool IsDecrypting { get; set; } = false;
        /// <summary>
        /// Tells the View if a file is being encrypted right now
        /// </summary>
        public bool Idling { get; set; } = true;

        #endregion

        #region Commands

        /// <summary>
        /// Command for encrypting the file
        /// </summary>
        public ICommand DecryptCancelCommand { get; set; }

        #endregion

        #region Private members

        /// <summary>
        /// Token for canceloing the decryption task
        /// </summary>
        private CancellationTokenSource Token;

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

        #region Command functions

        /// <summary>
        /// Function for the <see cref="DecryptCancelCommand"/> command
        /// </summary>
        /// <param name="o"></param>
        private async void DecryptCancelCommand_Function(object o)
        {
            // If a file is being decrypted...
            if (!IsDecrypting)
            {
                // Turn on decryption mode
                IsDecrypting = true;

                // Set the file size
                FileSize = new FileInfo(OriginalFileName).Length;

                // Create the cancellation token source
                Token = new CancellationTokenSource();

                // Encrypt the file
                var res = await DecryptFileAES(OriginalFileName, @$"{CurrentFilePath}\{CurrentFileName}",
                    ((IHavePassword)o).Password, DeleteOriginalFile, Token.Token);

                // Handle the recieved result
                HandleDecryptResult(res);
            }
            // Else...
            else
            {
                // Set state to busy
                Idling = false;

                // Show user that the decryption is being canceled
                DecryptionInformation = "Canceling...";

                // Cancel the decryption operation
                Token.Cancel();
            }
        }

        #endregion

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

            // Create and bind the Decrypt command to it's function
            DecryptCancelCommand = new ParameterizedRelayCommand(DecryptCancelCommand_Function);
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
                    ErrorTypes.FileNotFound       => "Could not find file: " + CurrentFileName,
                    ErrorTypes.DirectoryNotFound  => "Directory does not exist: " + CurrentFileName,
                    ErrorTypes.OperationCanceled  => "Decryption canceled",
                    _ => String.Empty
                };
                // Turn off decryption mode
                IsDecrypting = false;
                // If the decryption was canceled
                if (dr.Error.Equals(ErrorTypes.OperationCanceled))
                    // turn on idle state
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