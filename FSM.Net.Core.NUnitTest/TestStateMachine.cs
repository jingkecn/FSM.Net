using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM.Net.Standard;
using NUnit.Framework;
using Trie.Net.Standard;

namespace FSM.Net.Core.NUnitTest
{
    [TestFixture]
    public class TestStateMachine
    {
        [SetUp]
        public void SetUp()
        {
            StateMachine = new StateMachine();
        }

        private StateMachine StateMachine { get; set; }

        private static IEnumerable<IEnumerable<IState>> PresetS00 => new[] {new[] {S00}};
        private static IEnumerable<IEnumerable<IState>> PresetS01 => PresetS00.Concat(new[] {new[] {S01}});
        private static IEnumerable<IEnumerable<IState>> PresetS10 => PresetS01.Concat(new[] {new[] {S01, S10}});
        private static IEnumerable<IEnumerable<IState>> PresetS11 => PresetS10.Concat(new[] {new[] {S01, S11}});

        private static readonly IState S00 = new State(nameof(S00));
        private static readonly IState S01 = new State(nameof(S01));
        private static readonly IState S10 = new State(nameof(S10));
        private static readonly IState S11 = new State(nameof(S11));
        private static readonly IState StateNotExists = new State(nameof(StateNotExists));

        public static IEnumerable TestCaseAddState
        {
            get
            {
                yield return new TestCaseData(S00, null, null, new[] {S00}).Returns(true);
                yield return new TestCaseData(S01, null, null, new[] {S01}).Returns(true);
                yield return new TestCaseData(S10, S01, PresetS01, new[] {S01, S10}).Returns(true);
                yield return new TestCaseData(S11, S01, PresetS10, new[] {S01, S11}).Returns(true);
            }
        }

        public static IEnumerable TestCaseExists
        {
            get
            {
                yield return new TestCaseData(S00, PresetS00).Returns(true);
                yield return new TestCaseData(S01, PresetS01).Returns(true);
                yield return new TestCaseData(S10, PresetS10).Returns(true);
                yield return new TestCaseData(S11, PresetS11).Returns(true);
                yield return new TestCaseData(StateNotExists, PresetS11).Returns(false);
            }
        }

        public static IEnumerable TestCasePathTo
        {
            get
            {
                yield return new TestCaseData(S00, PresetS00, new[] {S00}).Returns(true);
                yield return new TestCaseData(S01, PresetS01, new[] {S01}).Returns(true);
                yield return new TestCaseData(S10, PresetS10, new[] {S01, S10}).Returns(true);
                yield return new TestCaseData(S11, PresetS11, new[] {S01, S11}).Returns(true);
            }
        }

        public static IEnumerable TestCaseSearch
        {
            get
            {
                yield return new TestCaseData(S00, PresetS00).Returns(true);
                yield return new TestCaseData(S01, PresetS01).Returns(true);
                yield return new TestCaseData(S10, PresetS10).Returns(true);
                yield return new TestCaseData(S11, PresetS11).Returns(true);
                yield return new TestCaseData(StateNotExists, PresetS11).Returns(false);
            }
        }

        public static IEnumerable TestCaseTransitionTo
        {
            get
            {
                yield return new TestCaseData(S00, new[] {S00}).Returns(true);
                yield return new TestCaseData(S01, new[] {S01}).Returns(false);
                yield return new TestCaseData(S01, new[] {S01, S10}).Returns(true);
                yield return new TestCaseData(S10, new[] {S01, S10}).Returns(true);
                yield return new TestCaseData(S11, new[] {S01, S11}).Returns(true);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestCaseAddState))]
        public bool TestAddState(IState state, IState parent,
            IEnumerable<IEnumerable<IState>> presets,
            IEnumerable<IState> expected)
        {
            if (presets != null)
                foreach (var preset in presets)
                    StateMachine.Insert(preset.ToArray());
            StateMachine.AddState(state, parent);
            return StateMachine.PathTo(state).SequenceEqual(expected);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseExists))]
        public bool TestExists(IState state, IEnumerable<IEnumerable<IState>> presets)
        {
            foreach (var preset in presets) StateMachine.Insert(preset.ToArray());
            return StateMachine.Contains(state);
        }

        [Test]
        [TestCaseSource(nameof(TestCasePathTo))]
        public bool TestPathTo(IState state, IEnumerable<IEnumerable<IState>> presets, IState[] expected)
        {
            foreach (var preset in presets) StateMachine.Insert(preset.ToArray());
            return StateMachine.PathTo(state).SequenceEqual(expected);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseSearch))]
        public bool TestSearch(IState state, IEnumerable<IEnumerable<IState>> presets)
        {
            foreach (var preset in presets) StateMachine.Insert(preset.ToArray());
            return StateMachine.Search(node => node.Value == state).SingleOrDefault() is Node<IState> target &&
                   target.Value == state;
        }

        [Test]
        [TestCaseSource(nameof(TestCaseTransitionTo))]
        public bool TestTransitionTo(IState state, IState[] expected)
        {
            StateMachine.AddState(S00);
            StateMachine.AddState(S01);
            StateMachine.AddState(S10, S01);
            StateMachine.AddState(S11, S01);
            StateMachine.TransitionTo(state);
            var actual = new List<IState>();
            var node = StateMachine.Root;
            do
            {
                var active = node.Children.Single(child => child.Value.IsActive);
                actual.Add(active.Value);
                node = active;
            } while (!node.IsEnd);

            return actual.SequenceEqual(expected);
        }
    }
}
