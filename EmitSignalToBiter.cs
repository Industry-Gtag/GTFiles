using System;
using UnityEngine;

// Token: 0x02000693 RID: 1683
public class EmitSignalToBiter : GTSignalEmitter
{
	// Token: 0x06002A03 RID: 10755 RVA: 0x000E293C File Offset: 0x000E0B3C
	public override void Emit()
	{
		if (this.onEdibleState == EmitSignalToBiter.EdibleState.None)
		{
			return;
		}
		if (!this.targetEdible)
		{
			return;
		}
		if (this.targetEdible.lastBiterActorID == -1)
		{
			return;
		}
		TransferrableObject.ItemStates itemState = this.targetEdible.itemState;
		if (itemState - TransferrableObject.ItemStates.State0 <= 1 || itemState == TransferrableObject.ItemStates.State2 || itemState == TransferrableObject.ItemStates.State3)
		{
			int num = (int)itemState;
			if ((this.onEdibleState & (EmitSignalToBiter.EdibleState)num) == (EmitSignalToBiter.EdibleState)num)
			{
				GTSignal.Emit(this.targetEdible.lastBiterActorID, this.signal, Array.Empty<object>());
			}
		}
	}

	// Token: 0x06002A04 RID: 10756 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void Emit(int targetActor)
	{
	}

	// Token: 0x06002A05 RID: 10757 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void Emit(params object[] data)
	{
	}

	// Token: 0x040036A2 RID: 13986
	[Space]
	public EdibleHoldable targetEdible;

	// Token: 0x040036A3 RID: 13987
	[Space]
	[SerializeField]
	private EmitSignalToBiter.EdibleState onEdibleState;

	// Token: 0x02000694 RID: 1684
	[Flags]
	private enum EdibleState
	{
		// Token: 0x040036A5 RID: 13989
		None = 0,
		// Token: 0x040036A6 RID: 13990
		State0 = 1,
		// Token: 0x040036A7 RID: 13991
		State1 = 2,
		// Token: 0x040036A8 RID: 13992
		State2 = 4,
		// Token: 0x040036A9 RID: 13993
		State3 = 8
	}
}
