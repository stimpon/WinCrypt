namespace WinCrypt
{
    /// <summary>
    /// Class that keeps track of the programs current state
    /// </summary>
    public static class ProgramState
    {
        /// <summary>
        /// Gets or sets a value indicating whether this program is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this program is running; otherwise, <c>false</c>.
        /// </value>
        public static bool IsRunning { get; set; } = false;
    }
}
