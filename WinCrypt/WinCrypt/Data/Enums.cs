/// <summary>
/// All pages that the program have
/// </summary>
public enum ApplicationPages
{
    /// <summary>
    /// This is a reference to the />
    /// </summary>
    EncryptPage = 0,

    /// <summary>
    /// This is a reference to the />
    /// </summary>
    DecryptPage = 1,
}

/// <summary>
/// Error types for cryptography results
/// </summary>
public enum ErrorTypes
{
    /// <summary>
    /// No errors
    /// </summary>
    None,

    /// <summary>
    /// The given key is incorrect
    /// </summary>
    WrongDecryptionKey,

    /// <summary>
    /// File was not found
    /// </summary>
    FileNotFound,

    /// <summary>
    /// Directory does not exist
    /// </summary>
    DirectoryNotFound
}