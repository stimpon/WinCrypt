namespace WinCrypt
{
    // Required namespaces
    using Microsoft.Win32;
    using System;
    using System.IO;

    public static class RegistryHelpers
    {
        /// <summary>
        /// Sets up the necessary registry keys and values
        /// 
        /// This function will first check if the keys exists and will create
        /// them if they don't exist
        /// </summary>
        public static void RegistrySetup()
        {
            try
            {
                // Get the registry subkey for this program
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("WinCrypt");
                // If it does not exist
                if (key == null)
                {
                    // Then create the registry key
                    var subKey = Registry.ClassesRoot.CreateSubKey("WinCrypt");
                    // Set default icon
                    subKey.CreateSubKey("DefaultIcon").SetValue("", Directory.GetCurrentDirectory() + @"\WinCrypt.exe,0");

                    // Create shell command
                    var shellKey = subKey.CreateSubKey("shell").CreateSubKey("Decrypt");
                    // Set command string
                    shellKey.SetValue("", "Decrypt");
                    // Set command icon
                    shellKey.SetValue("Icon", Directory.GetCurrentDirectory() + @"\WinCrypt.exe,0");
                    // Create command
                    shellKey.CreateSubKey("Command").SetValue("", @"""" + Directory.GetCurrentDirectory() + @"\WinCrypt.exe"" ""%1"" ""1""");
                }

                // Get the registry subkey for the encrypted file extension
                key = Registry.ClassesRoot.OpenSubKey(".encrypted");
                // If it does not exist
                if (key == null)
                {
                    // Create the .encrypted file extension registry key
                    var subKey = Registry.ClassesRoot.CreateSubKey(".encrypted");
                    // Accosiate the .encrypted file extension with this program
                    subKey.SetValue("", "WinCrypt");
                }

                // Get registry subkey for the Encrypt command
                key = Registry.ClassesRoot.OpenSubKey(@"*\shell", true);
                // If it does not exist
                if (key.OpenSubKey("Encrypt") == null)
                {
                    // Create the encrypt key
                    var subKey = key.CreateSubKey("Encrypt");
                    // Set Encrypt icon
                    subKey.SetValue("Icon", Directory.GetCurrentDirectory() + @"\WinCrypt.exe,0");
                    // Create command
                    var subsubkey = subKey.CreateSubKey("Command");
                    // Set command value
                    subsubkey.SetValue("", @"""" + Directory.GetCurrentDirectory() + @"\WinCrypt.exe"" ""%1"" ""0""");
                }
            }
            catch(Exception ex)
            {
                // If the setup faild, run the cleanup function
                RegistryCleanup();

                // And then throw an exception containing the exception message
                throw new System.Exception(ex.Message);
            }
        }

        /// <summary>
        /// Removes all registry keys and values associated with this program
        /// </summary>
        public static void RegistryCleanup()
        {
            // Try to access the registry and remove keys associated with this program
            try
            {
                // Open classes root in the registry
                using (RegistryKey key = Registry.ClassesRoot)
                {
                    // If WinCrypt exist
                    if (key.OpenSubKey("WinCrypt") != null) 
                        // Delete the Registry Key
                        key.DeleteSubKey("WinCrypt");
                    // If .encrypted exist
                    if (key.OpenSubKey(".encrypted") != null)
                        // Delete the Registry Key
                        key.DeleteSubKey(".encrypted");
                    // If Encrypt key exist
                    if (key.OpenSubKey(@"*\shell\Encrypt") != null)
                        // Delete the Registry Key
                        key.DeleteSubKey(@"*\shell\Encrypt");
                }
            }
            // If it fails
            catch (Exception ex)
            {
                // If cleanup failed, thrown an exception containing the exception message
                throw new Exception($"Cleanup failed: {ex.Message}");
            }
        }
    }
}
