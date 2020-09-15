namespace WinCrypt
{
    // Required namespaces
    using System;
    using System.IO;
    using System.Text;
    using System.Security;
    using System.Threading.Tasks;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Windows;

    /// <summary>
    /// Contains helpers for encryption and decryption
    /// </summary>
    public static class CryptographyHelpers
    {
        #region Public properties

        /// <summary>
        /// The memory buffer to use when preforming cryptograpgic operations
        /// </summary>
        public static int MemoryBuffer { get; set; } = 250000000;

        #endregion

        #region Encryption events

        /// <summary>
        /// Can be subscibed to to recieve encryption progress
        /// </summary>
        public static event EventHandler<long> EncryptionProgress = delegate { };
        /// <summary>
        /// Can be subscibed to to recieve encryption information
        /// </summary>
        public static event EventHandler<string> EncryptionInfo = delegate { };

        #endregion

        #region Decryption events

        /// <summary>
        /// Can be subscibed to to recieve encryption progress
        /// </summary>
        public static event EventHandler<long> DecryptionProgress = delegate { };
        /// <summary>
        /// Can be subscibed to to recieve decryption information
        /// </summary>
        public static event EventHandler<string> DecryptionInfo = delegate { };

        #endregion

        #region Functions

        /// <summary>
        /// Hashes a secure string using the Sha 256 algorithm and returns the bytes
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static byte[] HashSecureStringSHA256(SecureString password)
        {
            // Create a new hasher
            using (SHA256 Hasher = SHA256.Create())
            {
                // Get the string from the secure string and hash it, finaly return it
                return Hasher.ComputeHash(Encoding.Default.GetBytes(password.ToUnsecureString()));
            }
                 
        }

        /// <summary>
        /// Encrypt the specified file
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="pass">the encryption key</param>
        /// <param name="DeleteOriginalFile">True if the original file should be deleted after encryption</param>
        /// <returns></returns>
        public static async Task<CryptographicResult> EncryptFileAsync(
            string filePath, 
            string newFilePath,
            SecureString pass, 
            bool DeleteOriginalFile,
            CancellationToken cancelToken) {
            // Try to encrypt the file

            // Create the new file string
            string newFile = @$"{newFilePath}.encrypted";

            try
            {
                // Placeholders for the iv
                byte[] IV = new byte[16];

                // Create a random number generator safe for cryptography
                using (RandomNumberGenerator RNG = RandomNumberGenerator.Create())
                    // Generate a random IV
                    RNG.GetBytes(IV); // Generate a random 128 bit IV

                using (Aes AES = Aes.Create())
                {                 
                    // Set Aes properties
                    AES.KeySize = 256;
                    AES.Mode    = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;
                    AES.Key = HashSecureStringSHA256(pass);
                    AES.IV  = IV;            

                    using (FileStream Reader   = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (FileStream Writer   = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                    using (CryptoStream Stream = new CryptoStream(Writer, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // Write the IV to the begining of the file
                        await Writer.WriteAsync(IV, cancelToken);

                        // Send encryption information
                        EncryptionInfo(null, "Encrypting...");
                        // Keep reading bytes until the end of the file is hit
                        while (Reader.Position != Reader.Length)
                        {
                            // Placeholder for read bytes
                            byte[] bytes = new byte[MemoryBuffer];

                            // Read 512 bytes or the amount of bytes that are left
                            int read = await Reader.ReadAsync(bytes, 0, (Reader.Position + MemoryBuffer <= Reader.Length) ? MemoryBuffer : (int)(Reader.Length - Reader.Position), cancelToken);
                            // Resize the array if it is the last byte packet
                            if (read != MemoryBuffer) Array.Resize(ref bytes, read);

                            // Copy the read byte over to the cryptostream
                            await Stream.WriteAsync(bytes, cancelToken);

                            // Call update event
                            EncryptionProgress(null, Writer.Length - 16);
                        }
                    }
                    // If the original file should be deleted...
                    if (DeleteOriginalFile)
                        // Remove the original file
                        File.Delete(filePath);
                    // Else... nothing
                }
                // Return a True result
                return new CryptographicResult();
            }
            // If the encryption was canceled
            catch(OperationCanceledException)
            {
                // Remove the file
                File.Delete(newFile);

                // Send encryption information
                EncryptionInfo(null, "Encryption Canceled");
                // Call update event
                EncryptionProgress(null, 0);

                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.OperationCanceled };
            }
            catch (FileNotFoundException)
            {
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.FileNotFound };
            }
            catch (DirectoryNotFoundException)
            {
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.DirectoryNotFound };
            }
            catch (Exception)
            {
                return new CryptographicResult() { Error = ErrorTypes.DirectoryNotFound };
            }
        }

        /// <summary>
        /// Decrypts the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="encryptionKey">The full filepath to the encryption key.</param>
        /// <param name="encryptionIV">The full filepath to the encryption iv.</param>
        public static async Task<CryptographicResult> DecryptFileAES(
            string filePath, 
            string newFileName,
            SecureString pass,
            bool DeleteOriginalFile,
            CancellationToken cancelToken) {

            // Try to decryption the file
            try
            {
                using (Aes AES = Aes.Create())
                {
                    // Set Aes properties
                    AES.KeySize = 256;
                    AES.Mode    = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;
                    AES.Key = HashSecureStringSHA256(pass);
                    AES.IV  = new byte[16];

                    using (FileStream Reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        // Create a new placeholder for the IV
                        byte[] IV = new byte[16];
                        // Read the IV from the file
                        await Reader.ReadAsync(IV, 0, 16, cancelToken);
                        // Set the IV
                        AES.IV = IV;

                        // Can fail because of the cancellation token
                        try
                        {
                            // Open up a file writer and a crypto stream
                            using (FileStream Writer = new FileStream(newFileName, FileMode.Create, FileAccess.Write))
                            using (CryptoStream Stream = new CryptoStream(Writer, AES.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                // Send decryption message
                                DecryptionInfo(null, "Decrypting...");

                                // Keep reading bytes until the end of the file is hit
                                while (Reader.Position != Reader.Length)
                                {
                                    // Placeholder for the decrypted bytes
                                    byte[] bytes = new byte[MemoryBuffer];

                                    // Read bytes from input file
                                    int read = await Reader.ReadAsync(bytes, 0, (Reader.Position + MemoryBuffer <= Reader.Length) ? MemoryBuffer : (int)(Reader.Length - Reader.Position), cancelToken);
                                    // Resize the array if it is the last byte packet
                                    if (read != MemoryBuffer) Array.Resize(ref bytes, read);

                                    // Copy bytes to decryption stream
                                    await Stream.WriteAsync(bytes, cancelToken);

                                    // Send progress update
                                    DecryptionProgress(null, Writer.Length);
                                }
                            }
                        }
                        // We must check if the exception was caught because the decryption was canceled or 
                        // because there was a problem with the decryption
                        catch
                        {
                            // If the exception was caught because the operation was canceled...
                            if (cancelToken.IsCancellationRequested) 
                                // Throw a operation canceled exception instead of a cryptographic exception
                                throw new OperationCanceledException();
                            // Else...
                            else 
                                // Just throw a cryptographic exception
                                throw new CryptographicException();
                        }

                    }
                    // If the original file should be deleted
                    if(DeleteOriginalFile)
                        // Delete the encrypted file
                        File.Delete(filePath);
                }

                // Return a True result
                return new CryptographicResult();
            }
            // If the decryption process was canceled
            catch(OperationCanceledException)
            {
                // Remove the decryption file
                File.Delete(newFileName);

                // Send progress update
                DecryptionProgress(null, 0);
                // Send canceled complete message
                DecryptionInfo(null, "Decryption canceled");

                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.OperationCanceled };
            }
            //If decryption fails...
            catch (CryptographicException)
            {
                // Delete the created file
                if (!String.IsNullOrEmpty(newFileName)) File.Delete(newFileName);
                // Send progress update
                DecryptionProgress(null, 0);
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.WrongDecryptionKey };
            }
            catch (FileNotFoundException)
            {
                // Delete the created file
                if (!String.IsNullOrEmpty(newFileName)) File.Delete(newFileName);
                // Send progress update
                DecryptionProgress(null, 0);
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.FileNotFound };
            }
            catch (DirectoryNotFoundException)
            {
                // Delete the created file
                if (!String.IsNullOrEmpty(newFileName)) File.Delete(newFileName);
                // Send progress update
                DecryptionProgress(null, 0);
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.DirectoryNotFound };
            }
        }

        #endregion

    }
}
