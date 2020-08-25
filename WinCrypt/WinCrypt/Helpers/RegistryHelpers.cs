namespace WinCrypt
{
    // Required namespaces
    using Microsoft.Win32;
    using System;
    using System.IO;
    using System.Windows;

    public static class RegistryHelpers
    {
        /// <summary>
        /// Sets up the necessary registry keys and values
        /// =================================================================
        /// This function will first check if the keys exists and will create
        /// them if they don't exist
        /// </summary>
        public static void RegistrySetup()
        {
            try
            {
                #region Check keys

                // Get the registry subkey for the this program
                RegistryKey keyWIN = Registry.ClassesRoot.OpenSubKey("WinCrypt");
                // Get the registry subkey for the encrypted file extension
                RegistryKey keyENC = Registry.ClassesRoot.OpenSubKey(".encrypted");
                // Get registry subkey for the Encrypt command
                RegistryKey keyCOM = Registry.ClassesRoot.OpenSubKey(@"*\shell", true).OpenSubKey("Encrypt");

                #endregion

                // Check if any keys are null
                if (keyWIN == null || keyENC == null || keyCOM == null)
                {
                    // Ask user if he/she wants to add the registry keys right now
                    var userResult = MessageBox.Show("You are missing necessary registry keys, this program will not run correctly without them. Do you want to create them now?", "Missing registry keys", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    // Check the user choice
                    switch (userResult)
                    {
                        case MessageBoxResult.Yes:
                            // Remove ecisting keys
                            RegistryCleanup();

                            #region Create all necessary subkeys
                            // Then create the registry key
                            var subKey1 = Registry.ClassesRoot.CreateSubKey("WinCrypt");
                            // Set default icon
                            subKey1.CreateSubKey("DefaultIcon").SetValue("", Directory.GetCurrentDirectory() + @"\WinCrypt.exe,0");

                            // Create shell command
                            var shellKey = subKey1.CreateSubKey("shell").CreateSubKey("Decrypt");
                            // Set command string
                            shellKey.SetValue("", "Decrypt");
                            // Set command icon
                            shellKey.SetValue("Icon", Directory.GetCurrentDirectory() + @"\WinCrypt.exe,0");
                            // Create command
                            shellKey.CreateSubKey("Command").SetValue("", @"""" + Directory.GetCurrentDirectory() + @"\WinCrypt.exe"" ""%1"" ""1""");

                            // Create the .encrypted file extension registry key
                            var subKey2 = Registry.ClassesRoot.CreateSubKey(".encrypted");
                            // Accosiate the .encrypted file extension with this program
                            subKey2.SetValue("", "WinCrypt");

                            // Create the encrypt key
                            var subKey3 = Registry.ClassesRoot.OpenSubKey(@"*\shell", true).CreateSubKey("Encrypt");
                            // Set Encrypt icon
                            subKey3.SetValue("Icon", Directory.GetCurrentDirectory() + @"\WinCrypt.exe,0");
                            // Create command
                            var subsubkey = subKey3.CreateSubKey("Command");
                            // Set command value
                            subsubkey.SetValue("", @"""" + Directory.GetCurrentDirectory() + @"\WinCrypt.exe"" ""%1"" ""0""");
                            #endregion

                            break;
                        // Just return if the user chooses not to add the registry keys
                        default: return;
                    }
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
                        key.DeleteSubKeyTree("WinCrypt");
                    // If .encrypted exist
                    if (key.OpenSubKey(".encrypted") != null)
                        // Delete the Registry Key
                        key.DeleteSubKeyTree(".encrypted");
                    // If Encrypt key exist
                    if (key.OpenSubKey(@"*\shell\Encrypt") != null)
                        // Delete the Registry Key
                        key.DeleteSubKeyTree(@"*\shell\Encrypt");
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
