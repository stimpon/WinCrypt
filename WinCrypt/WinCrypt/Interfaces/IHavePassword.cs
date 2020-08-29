namespace WinCrypt
{
    // Required namespaces
    using System.Security;

    /// <summary>
    /// Interface for an object that contain a password
    /// </summary>
    public interface IHavePassword
    {
        /// <summary>
        /// The password
        /// </summary>
        SecureString Password { get; } 
    }
}
