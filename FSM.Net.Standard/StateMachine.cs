using System.Collections.Generic;
using System.Linq;
using Trie.Net.Standard;

namespace FSM.Net.Standard
{
    public partial class StateMachine
    {
        public void AddState(IState state, IState parent = null)
        {
            if (parent == null)
            {
                Insert(state);
            }
            else
            {
                var path = PathTo(parent).ToList();
                Remove(path.ToArray());
                path.Add(state);
                Insert(path.ToArray());
            }
        }
    }

    #region Implementation (Trie)

    public partial class StateMachine : Trie<IState>
    {
        public void Activate(IState state)
        {
            var current = Search(node => node.IsEnd && node.Value.IsActive).SingleOrDefault();
            var toStates = PathTo(state).ToList();
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

        public IEnumerable<IState> PathTo(IState state)
        {
            return PathTo(node => node.Value == state).Select(node => node.Value);
        }

        public bool Exists(IState state)
        {
            return Search(state) != null;
        }

        public Node<IState> Search(IState state)
        {
            return Search(node => node.Value == state).SingleOrDefault();
        }
    }

    #endregion
}
