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

        private static IEnumerable<IState> States => new List<IState> {S00, S01, S10, S11};

        public static IEnumerable TestCaseActivate
        {
            get
            {
                yield return new TestCaseData(new List<IState> {S00});
                yield return new TestCaseData(new List<IState> {S01, S10});
                yield return new TestCaseData(new List<IState> {S01, S11});
            }
        }

        public static IEnumerable TestCaseActivateAgain
        {
            get
            {
                yield return new TestCaseData(new List<IState> {S00}, new List<IState> {S01, S10});
                yield return new TestCaseData(new List<IState> {S01, S10}, new List<IState> {S01, S11});
                yield return new TestCaseData(new List<IState> {S01, S11}, new List<IState> {S00});
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
        public void TestActivate(List<IState> states)
        {
            StateMachine.Insert(S00);
            StateMachine.Insert(S01, S10);
            StateMachine.Insert(S01, S11);
            StateMachine.Activate(states.ToArray());
            Assert.IsTrue(states.All(state => state.IsActive));
            Assert.IsFalse(
                (from state in States where !states.Contains(state) select state).All(state => state.IsActive));
        }

        [Test]
        [TestCaseSource(nameof(TestCaseActivateAgain))]
        public void TestActivateAgain(List<IState> firstStates, List<IState> secondStates)
        {
            StateMachine.Insert(S00);
            StateMachine.Insert(S01, S10);
            StateMachine.Insert(S01, S11);
            StateMachine.Activate(firstStates.ToArray());
            Assert.IsTrue(firstStates.All(state => state.IsActive));
            Assert.IsFalse(
                (from state in States where !firstStates.Contains(state) select state).All(state => state.IsActive));
            StateMachine.Activate(secondStates.ToArray());
            Assert.IsTrue(secondStates.All(state => state.IsActive));
            Assert.IsFalse(
                (from state in States where !secondStates.Contains(state) select state).All(state => state.IsActive));
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
