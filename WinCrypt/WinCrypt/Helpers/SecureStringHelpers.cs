namespace WinCrypt
{
    // Required namespaces
    using System;
    using System.Security;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Helpers for a <see cref="SecureString"/>
    /// </summary>
    public static class SecureStringHelpers
    {
        /// <summary>
        /// Gets the string from a secure string
        /// </summary>
        /// <param name="SS">The <see cref="SecureString"/></param>
        /// <returns></returns>
        public static string ToUnsecureString(this SecureString SS)
        {
            // Check if the secure string is null
            if (SS == null) 
                // Return a null string
                return null;

            // Pointer for an unmanaged string
            var unmanaged = IntPtr.Zero;

            try
            {
                // Get string from the secure string
                unmanaged = Marshal.SecureStringToGlobalAllocUnicode(SS);

                // Get string from pointer and return it
                return Marshal.PtrToStringUni(unmanaged);
            }
            finally
            {
                // Clear memeory
                Marshal.ZeroFreeGlobalAllocUnicode(unmanaged);
            }
        }
    }
}
