namespace WinCrypt
{
    /// <summary>
    /// Required namespaces
    /// </summary>
    #region Namespaces
    using System;
    using System.Windows.Input;
    #endregion

    /// <summary>
    /// A Parameterized RelayCommand
    /// </summary>
    public class ParameterizedRelayCommand : ICommand
    {      
        public event EventHandler CanExecuteChanged
        {
            add    { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #region Private members

        /// <summary>
        /// Command action
        /// </summary>
        private Action<object> CommandAction;

        /// <summary>
        /// CanExectute validation predicate
        /// </summary>
        private Predicate<bool> CommandPredicate;

        #endregion

        /// <summary>
        /// Constructor that takes a predicate and an action
        /// </summary>
        /// <param name="CommandAction"></param>
        /// <param name="CommandPredicate"></param>
        public ParameterizedRelayCommand(
            Action<object> CommandAction, 
            Predicate<bool> CommandPredicate) {

            // Set the command action
            this.CommandAction    = CommandAction;
            // Set the command predicate
            this.CommandPredicate = CommandPredicate;
        }

        /// <summary>
        /// Constructor that takes an action and sets CanExecute to true at all times
        /// </summary>
        /// <param name="CommandAction"></param>
        /// <param name="CommandPredicate"></param>
        public ParameterizedRelayCommand(Action<object> CommandAction)
        {
            // Set the command action
            this.CommandAction = CommandAction;
            // Set the command predicate
            this.CommandPredicate = (o) => { return true; };
        }

        #region Functions

        /// <summary>
        /// Determines if the command can be executed
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
            =>
            true;

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
            =>
            CommandAction(parameter);

        #endregion
    }
}
