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

        private static readonly IState S00 = new State(nameof(S00));
        private static readonly IState S01 = new State(nameof(S01));
        private static readonly IState S10 = new State(nameof(S10));
        private static readonly IState S11 = new State(nameof(S11));
        private static readonly IState StateNotExists = new State(nameof(StateNotExists));

        public static IEnumerable TestCaseActivate
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

        public static IEnumerable TestCaseAddState
        {
            get
            {
                yield return new TestCaseData(S00, null, null, new[] {S00}).Returns(true);
                yield return new TestCaseData(S01, null, null, new[] {S01}).Returns(true);
                yield return new TestCaseData(S10, S01, new[] {new[] {S00}, new[] {S01}}, new[] {S01, S10})
                    .Returns(true);
                yield return new TestCaseData(S11, S01, new[] {new[] {S00}, new[] {S01}, new[] {S01, S10}},
                    new[] {S01, S11}).Returns(true);
            }
        }

        public static IEnumerable TestCaseAddS00
        {
            get { yield return new TestCaseData(S00, null, new[] {S00}).Returns(true); }
        }

        public static IEnumerable TestCaseAddS01
        {
            get { yield return new TestCaseData(S01, null, new[] {S01}).Returns(true); }
        }

        public static IEnumerable TestCaseAddS10
        {
            get { yield return new TestCaseData(S10, S01, new[] {S01, S10}).Returns(true); }
        }

        public static IEnumerable TestCaseAddS11
        {
            get { yield return new TestCaseData(S11, S01, new[] {S01, S11}).Returns(true); }
        }

        public static IEnumerable TestCaseBuildPath
        {
            get
            {
                yield return new TestCaseData(S00, new[] {S00}).Returns(true);
                yield return new TestCaseData(S01, new[] {S01}).Returns(true);
                yield return new TestCaseData(S10, new[] {S01, S10}).Returns(true);
                yield return new TestCaseData(S11, new[] {S01, S11}).Returns(true);
            }
        }

        public static IEnumerable TestCaseExists
        {
            get
            {
                yield return new TestCaseData(S00).Returns(true);
                yield return new TestCaseData(S01).Returns(true);
                yield return new TestCaseData(S10).Returns(true);
                yield return new TestCaseData(S11).Returns(true);
                yield return new TestCaseData(StateNotExists).Returns(false);
            }
        }

        public static IEnumerable TestCaseSearch
        {
            get
            {
                yield return new TestCaseData(S00).Returns(true);
                yield return new TestCaseData(S01).Returns(true);
                yield return new TestCaseData(S10).Returns(true);
                yield return new TestCaseData(S11).Returns(true);
                yield return new TestCaseData(StateNotExists).Returns(false);
            }
        }

        private StateMachine StateMachine { get; set; }

        [Test]
        [TestCaseSource(nameof(TestCaseActivate))]
        public bool TestActivate(IState state, IState[] expected)
        {
            StateMachine.AddState(S00);
            StateMachine.AddState(S01);
            StateMachine.AddState(S10, S01);
            StateMachine.AddState(S11, S01);
            StateMachine.Activate(state);
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

        [Test]
        [TestCaseSource(nameof(TestCaseAddState))]
        public bool TestAddState(IState state, IState parent, IEnumerable<IEnumerable<IState>> presets,
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
        public bool TestExists(IState state)
        {
            StateMachine.AddState(S00);
            StateMachine.AddState(S01);
            StateMachine.AddState(S10, S01);
            StateMachine.AddState(S11, S01);
            return StateMachine.Exists(state);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseBuildPath))]
        public bool TestPathTo(IState state, IState[] expected)
        {
            StateMachine.AddState(S00);
            StateMachine.AddState(S01);
            StateMachine.AddState(S10, S01);
            StateMachine.AddState(S11, S01);
            return StateMachine.PathTo(state).SequenceEqual(expected);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseSearch))]
        public bool TestSearch(IState state)
        {
            StateMachine.AddState(S00);
            StateMachine.AddState(S01);
            StateMachine.AddState(S10, S01);
            StateMachine.AddState(S11, S01);
            return StateMachine.Search(state) is Node<IState> node && node.Value == state;
        }
    }
}
