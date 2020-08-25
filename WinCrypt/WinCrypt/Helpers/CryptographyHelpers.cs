namespace WinCrypt
{
    // Required namespaces
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
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
        public static int MemoryBuffer { get; set; } = 10000000;

        #endregion

        #region Encryption events

        /// <summary>
        /// Can be subscibed to to recieve encryption progress
        /// </summary>
        public static event EventHandler<long> EncryptionProgress = delegate { };
        /// <summary>
        /// Can be subscibed to to recieve encryption information
        /// </summary>
        public static event EventHandler<string> EncryptionInfo= delegate { };

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
        /// Encrypt the specified file
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <param name="pass">The hashed password to use for encryption</param>
        /// <param name="DeleteOriginalFile">True if the original file should be deleted after encryption</param>
        /// <returns></returns>
        public static async Task<CryptographicResult> EncryptFileAES(
            string filePath, 
            string newFilePath,
            byte[] pass, 
            bool DeleteOriginalFile = true)
        {
            // Try to encrypt the file
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
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;
                    AES.Key = pass;
                    AES.IV = IV;

                    // Create the new file string
                    string newFile = @$"{newFilePath}.encrypted";                

                    using (FileStream Reader   = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (FileStream Writer   = new FileStream(newFile, FileMode.Create, FileAccess.Write))
                    using (CryptoStream Stream = new CryptoStream(Writer, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        // Send encryption information
                        EncryptionInfo(null, "Calculating MD5 checksum...");

                        // Calculate the MD5 checksum of the file
                        var Checksum = await Task.Run(() => {
                            // Create a MD5 hasher
                            using (MD5 md5 = MD5.Create())
                            using (Stream s = File.OpenRead(filePath))
                                // Comnpute checksum
                                return Task.FromResult(md5.ComputeHash(s));
                        });

                        // Save the checksum in the file
                        await Writer.WriteAsync(Checksum);

                        // Write the IV to the begining of the file
                        await Writer.WriteAsync(IV);

                        // Send encryption information
                        EncryptionInfo(null, "Encrypting...");
                        // Keep reading bytes until the end of the file is hit
                        while (Reader.Position != Reader.Length)
                        {
                            // Placeholder for read bytes
                            byte[] bytes = new byte[MemoryBuffer];

                            // Read 512 bytes or the amount of bytes that are left
                            int read = await Reader.ReadAsync(bytes, 0, (Reader.Position + MemoryBuffer <= Reader.Length) ? MemoryBuffer : (int)(Reader.Length - Reader.Position));
                            // Resize the array if it is the last byte packet
                            if (read != MemoryBuffer) Array.Resize(ref bytes, read);

                            // Copy the read byte over to the cryptostream
                            await Stream.WriteAsync(bytes);

                            // Call update event
                            EncryptionProgress(null, Writer.Length - 32);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            byte[] pass, bool 
            DeleteOriginalFile = true)
        {
            // If this is not empty in a catch then the file was created so remove it
            string newFile = String.Empty;

            // Try to decryption the file
            try
            {
                using (Aes AES = Aes.Create())
                {
                    // Set Aes properties
                    AES.KeySize = 256;
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;
                    AES.Key = pass;
                    AES.IV = new byte[16];

                    using (FileStream Reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        // Create a new placeholder for the IV
                        byte[] IV = new byte[16];
                        byte[] Signature = new byte[16];
                        // Read the signature from the file
                        Reader.Read(Signature, 0, 16);
                        // Read the IV from the file
                        Reader.Read(IV, 0, 16);
                        // Set the IV
                        AES.IV = IV;
                        // Create the new file string
                        newFile = @$"{new FileInfo(filePath).DirectoryName}\{Path.GetFileNameWithoutExtension(filePath)}";

                        using (FileStream Writer = new FileStream(newFile, FileMode.Create, FileAccess.Write))
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
                                int read = await Reader.ReadAsync(bytes, 0, (Reader.Position + MemoryBuffer <= Reader.Length) ? MemoryBuffer : (int)(Reader.Length - Reader.Position));
                                // Resize the array if it is the last byte packet
                                if (read != MemoryBuffer) Array.Resize(ref bytes, read);

                                // Copy bytes to decryption stream
                                await Stream.WriteAsync(bytes);

                                // Send progress update
                                DecryptionProgress(null, Writer.Length);
                            }
                        }

                        // Send decryption message
                        DecryptionInfo(null, "Comparing MD5 checksum...");

                        // Todo: Compare signatures to make sure the encryption was successful      
                        // Get hash from the decrypted file
                        var DecryptedSingature = await Task.Run(() =>
                        {
                            // Create a new MD5 hasher
                            using (MD5 md5 = MD5.Create())
                            using (Stream s = File.OpenRead(newFile))
                                // Return the computed hash
                                return md5.ComputeHash(s);
                        });

                        // Compare the 2 signatures
                        if (Convert.ToBase64String(DecryptedSingature).CompareTo(Convert.ToBase64String(Signature)) != 0)
                            throw new CryptographicException();

                    }
                    // If the original file should be deleted
                    if(DeleteOriginalFile)
                        // Delete the encrypted file
                        File.Delete(filePath);
                }

                // Return a True result
                return new CryptographicResult();
            }
            // If decryption fails...
            catch (CryptographicException)
            {
                // Delete the created file
                if (!String.IsNullOrEmpty(newFile)) File.Delete(newFile);
                // Send progress update
                DecryptionProgress(null, 0);
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.WrongDecryptionKey };
            }
            catch (FileNotFoundException)
            {
                // Delete the created file
                if (!String.IsNullOrEmpty(newFile)) File.Delete(newFile);
                // Send progress update
                DecryptionProgress(null, 0);
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.FileNotFound };
            }
            catch (DirectoryNotFoundException)
            {
                // Delete the created file
                if (!String.IsNullOrEmpty(newFile)) File.Delete(newFile);
                // Send progress update
                DecryptionProgress(null, 0);
                // Return a false result
                return new CryptographicResult() { Error = ErrorTypes.DirectoryNotFound };
            }
        }

        #endregion

    }
}
