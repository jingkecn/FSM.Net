using System.Diagnostics;
using System.Windows.Input;

namespace FSM.Net.Standard
{
    /// <summary>
    ///     The class implementing a state in a state machine.
    /// </summary>
    public class State : IState
    {
        public State(string name = null)
        {
            Name = name;
        }

        private string Name { get; }

        /// <inheritdoc cref="IState" />
        public bool IsActive { get; private set; }

        /// <inheritdoc cref="IState" />
        public virtual void Enter()
        {
#if DEBUG
            Debug.WriteLine($"{Name}: Entered");
#endif
            IsActive = true;
        }

        /// <inheritdoc cref="IState" />
        public virtual bool Execute(ICommand command)
        {
            return false;
        }

        /// <inheritdoc cref="IState" />
        public virtual void Exit()
        {
#if DEBUG
            Debug.WriteLine($"{Name}: Exited");
#endif
            IsActive = false;
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({nameof(Name)}={Name}, {nameof(IsActive)}={IsActive})";
        }
    }
}
