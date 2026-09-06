using System;
using UnityEngine;

// Token: 0x02000784 RID: 1924
public class SimpleStateMachine<State> where State : Enum
{
	// Token: 0x060030CD RID: 12493 RVA: 0x00108E53 File Offset: 0x00107053
	public void Setup(State initialState, Action<State> onStateStart, Action<State> onStateEnd, Action<State> onStateUpdate)
	{
		this.onStateStart = onStateStart;
		this.onStateEnd = onStateEnd;
		this.onStateUpdate = onStateUpdate;
		this.stateStartTime = Time.timeAsDouble;
		this.currState = initialState;
		if (onStateStart != null)
		{
			onStateStart(this.currState);
		}
	}

	// Token: 0x060030CE RID: 12494 RVA: 0x00108E8C File Offset: 0x0010708C
	public void Update()
	{
		Action<State> action = this.onStateUpdate;
		if (action == null)
		{
			return;
		}
		action(this.currState);
	}

	// Token: 0x060030CF RID: 12495 RVA: 0x00108EA4 File Offset: 0x001070A4
	public void SetState(State state, bool force = false)
	{
		if (!force && state.Equals(this.currState))
		{
			return;
		}
		Action<State> action = this.onStateEnd;
		if (action != null)
		{
			action(this.currState);
		}
		this.currState = state;
		this.stateStartTime = Time.timeAsDouble;
		Action<State> action2 = this.onStateStart;
		if (action2 == null)
		{
			return;
		}
		action2(this.currState);
	}

	// Token: 0x060030D0 RID: 12496 RVA: 0x00108F0E File Offset: 0x0010710E
	public State GetState()
	{
		return this.currState;
	}

	// Token: 0x060030D1 RID: 12497 RVA: 0x00108F16 File Offset: 0x00107116
	public double GetStateStartTime()
	{
		return this.stateStartTime;
	}

	// Token: 0x060030D2 RID: 12498 RVA: 0x00108F1E File Offset: 0x0010711E
	public bool IsStateFinished(double currTime, float stateDuration)
	{
		return currTime >= this.stateStartTime + (double)stateDuration;
	}

	// Token: 0x04003E72 RID: 15986
	private State currState;

	// Token: 0x04003E73 RID: 15987
	private double stateStartTime;

	// Token: 0x04003E74 RID: 15988
	private Action<State> onStateStart;

	// Token: 0x04003E75 RID: 15989
	private Action<State> onStateEnd;

	// Token: 0x04003E76 RID: 15990
	private Action<State> onStateUpdate;
}
