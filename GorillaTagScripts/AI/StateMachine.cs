using System;
using System.Collections.Generic;

namespace GorillaTagScripts.AI
{
	// Token: 0x0200107B RID: 4219
	public class StateMachine
	{
		// Token: 0x06006951 RID: 26961 RVA: 0x0021E9A4 File Offset: 0x0021CBA4
		public void Tick()
		{
			StateMachine.Transition transition = this.GetTransition();
			if (transition != null)
			{
				this.SetState(transition.To);
			}
			IState currentState = this._currentState;
			if (currentState == null)
			{
				return;
			}
			currentState.Tick();
		}

		// Token: 0x06006952 RID: 26962 RVA: 0x0021E9D8 File Offset: 0x0021CBD8
		public void SetState(IState state)
		{
			if (state == this._currentState)
			{
				return;
			}
			IState currentState = this._currentState;
			if (currentState != null)
			{
				currentState.OnExit();
			}
			this._currentState = state;
			this._transitions.TryGetValue(this._currentState.GetType(), out this._currentTransitions);
			if (this._currentTransitions == null)
			{
				this._currentTransitions = StateMachine.EmptyTransitions;
			}
			this._currentState.OnEnter();
		}

		// Token: 0x06006953 RID: 26963 RVA: 0x0021EA42 File Offset: 0x0021CC42
		public IState GetState()
		{
			return this._currentState;
		}

		// Token: 0x06006954 RID: 26964 RVA: 0x0021EA4C File Offset: 0x0021CC4C
		public void AddTransition(IState from, IState to, Func<bool> predicate)
		{
			List<StateMachine.Transition> list;
			if (!this._transitions.TryGetValue(from.GetType(), out list))
			{
				list = new List<StateMachine.Transition>();
				this._transitions[from.GetType()] = list;
			}
			list.Add(new StateMachine.Transition(to, predicate));
		}

		// Token: 0x06006955 RID: 26965 RVA: 0x0021EA93 File Offset: 0x0021CC93
		public void AddAnyTransition(IState state, Func<bool> predicate)
		{
			this._anyTransitions.Add(new StateMachine.Transition(state, predicate));
		}

		// Token: 0x06006956 RID: 26966 RVA: 0x0021EAA8 File Offset: 0x0021CCA8
		private StateMachine.Transition GetTransition()
		{
			foreach (StateMachine.Transition transition in this._anyTransitions)
			{
				if (transition.Condition())
				{
					return transition;
				}
			}
			foreach (StateMachine.Transition transition2 in this._currentTransitions)
			{
				if (transition2.Condition())
				{
					return transition2;
				}
			}
			return null;
		}

		// Token: 0x040078F2 RID: 30962
		private IState _currentState;

		// Token: 0x040078F3 RID: 30963
		private Dictionary<Type, List<StateMachine.Transition>> _transitions = new Dictionary<Type, List<StateMachine.Transition>>();

		// Token: 0x040078F4 RID: 30964
		private List<StateMachine.Transition> _currentTransitions = new List<StateMachine.Transition>();

		// Token: 0x040078F5 RID: 30965
		private List<StateMachine.Transition> _anyTransitions = new List<StateMachine.Transition>();

		// Token: 0x040078F6 RID: 30966
		private static List<StateMachine.Transition> EmptyTransitions = new List<StateMachine.Transition>(0);

		// Token: 0x0200107C RID: 4220
		private class Transition
		{
			// Token: 0x17000A01 RID: 2561
			// (get) Token: 0x06006959 RID: 26969 RVA: 0x0021EB8A File Offset: 0x0021CD8A
			public Func<bool> Condition { get; }

			// Token: 0x17000A02 RID: 2562
			// (get) Token: 0x0600695A RID: 26970 RVA: 0x0021EB92 File Offset: 0x0021CD92
			public IState To { get; }

			// Token: 0x0600695B RID: 26971 RVA: 0x0021EB9A File Offset: 0x0021CD9A
			public Transition(IState to, Func<bool> condition)
			{
				this.To = to;
				this.Condition = condition;
			}
		}
	}
}
