using System.Linq;
using Trie.Net.Standard;

namespace FSM.Net.Standard
{
    public class StateMachine : Trie<IState>
    {
        public void Activate(params IState[] states)
        {
            if (!Exists(states)) return;
            var node = Root;
            // Find deepest active state.
            while (!node.IsEnd && node.Children.Any(child => child.Value.IsActive))
                node = node.Children.Single(child => child.Value.IsActive);
            // Exit active states that are not in the target states path.
            while (node != Root && !states.Contains(node.Value))
            {
                node.Value.Exit();
                node = node.Parent;
            }

            // Enter the rest states of the target states path.
            node = Root;
            foreach (var state in states)
            {
                node = node.Children.Single(child => child.Value == state);
                if (!node.Value.IsActive) node.Value.Enter();
            }
        }
    }
}
