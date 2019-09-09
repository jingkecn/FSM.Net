﻿using System.Collections.Generic;
using System.Linq;
using Trie.Net.Standard;

namespace FSM.Net.Standard
{
    public partial class StateMachine
    {
        public void AddState(IState state, IState parent = null)
        {
            var path = new List<IState>();
            if (parent != null)
            {
                path.AddRange(PathTo(parent));
                Remove(path.ToArray());
            }

            path.Add(state);
            Insert(path.ToArray());
        }

        public void TransitionTo(IState state)
        {
            var current = Search(node => node.IsEnd && node.Value.IsActive).Single();
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

            var toNode = Search(node => node.Value == state).Single();
            while (toNode != null && !toNode.IsEnd)
            {
                var child = toNode.Children.First();
                child.Value.Enter();
                toNode = child;
            }
        }
    }

    #region Implementation (Trie)

    public partial class StateMachine : Trie<IState>
    {
        public bool Contains(IState state)
        {
            return PathTo(state) != null;
        }

        public IEnumerable<IState> PathTo(IState state)
        {
            return PathTo(node => node.Value == state).SingleOrDefault()?.Select(node => node.Value);
        }
    }

    #endregion
}
