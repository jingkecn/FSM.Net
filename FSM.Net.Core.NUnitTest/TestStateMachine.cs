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
            StateMachine.Insert(S00);
            StateMachine.Insert(S01, S10);
            StateMachine.Insert(S01, S11);
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
        [TestCaseSource(nameof(TestCaseBuildPath))]
        public bool TestBuildPathTo(IState state, IState[] expected)
        {
            StateMachine.Insert(S00);
            StateMachine.Insert(S01, S10);
            StateMachine.Insert(S01, S11);
            return StateMachine.BuildPathTo(state).SequenceEqual(expected);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseExists))]
        public bool TestExists(IState state)
        {
            StateMachine.Insert(S00);
            StateMachine.Insert(S01, S10);
            StateMachine.Insert(S01, S11);
            return StateMachine.Exists(state);
        }

        [Test]
        [TestCaseSource(nameof(TestCaseSearch))]
        public bool TestSearch(IState state)
        {
            StateMachine.Insert(S00);
            StateMachine.Insert(S01, S10);
            StateMachine.Insert(S01, S11);
            return StateMachine.Search(state) is Node<IState> node && node.Value == state;
        }
    }
}
