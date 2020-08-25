namespace WinCrypt
{
    /// <summary>
    /// Result model for a cryptographic procedure
    /// </summary>
    public class CryptographicResult
    {
        /// <summary>
        /// True if decryption worked
        /// </summary>
        public bool OK { get => Error == ErrorTypes.None; }

        /// <summary>
        /// 
        /// </summary>
        public ErrorTypes Error { get; set; } = ErrorTypes.None;
    }
}
