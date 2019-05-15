using System.Windows.Input;

namespace FSM.Net.Standard
{
    /// <summary>
    ///     The interface for implementing a state in a state machine.
    /// </summary>
    public interface IState
    {
        /// <summary>
        ///     <code>true</code> if a state is activated, otherwise <code>false</code>.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        ///     Called when a state is entered.
        /// </summary>
        void Enter();

        /// <summary>
        ///     Called when a command is to be executed by the state machine.
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns><code>true</code> if execution has completed, otherwise <code>false</code></returns>
        bool Execute(ICommand command);

        /// <summary>
        ///     Called when a state is exited.
        /// </summary>
        void Exit();
    }
}
