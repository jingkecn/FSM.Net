using System.Collections.Generic;
using System.Linq;
using Trie.Net.Standard;

namespace FSM.Net.Standard
{
    public class StateMachine : Trie<IState>
    {
        public void Activate(IState state)
        {
            var current = Search(node => node.IsEnd && node.Value.IsActive).SingleOrDefault();
            var toStates = BuildPathTo(state).ToList();
            while (current != null && current != Root && !toStates.Contains(current.Value))
            {
                current.Value.Exit();
                current = current.Parent;
            }

            toStates.ForEach(toState =>
            {
                if (!toState.IsActive) toState.Enter();
            });

            var toNode = Search(state);
            while (toNode != null && !toNode.IsEnd)
            {
                var child = toNode.Children.FirstOrDefault();
                child?.Value?.Enter();
                toNode = child;
            }
        }

        public IEnumerable<IState> BuildPathTo(IState state)
        {
            var stack = new Stack<IState>();
            var node = Search(state);
            while (node != null && node != Root)
            {
                stack.Push(node.Value);
                node = node.Parent;
            }

            return stack.ToArray();
        }

        public bool Exists(IState state)
        {
            return BuildPathTo(state).Count() != 0;
        }

        public Node<IState> Search(IState state)
        {
            return Search(node => node.Value == state).SingleOrDefault();
        }
    }
}
