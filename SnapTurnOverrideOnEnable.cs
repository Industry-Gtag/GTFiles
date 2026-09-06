using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x02000E0C RID: 3596
public class SnapTurnOverrideOnEnable : MonoBehaviour, ISnapTurnOverride
{
	// Token: 0x0600580C RID: 22540 RVA: 0x001CAE50 File Offset: 0x001C9050
	private void OnEnable()
	{
		if (this.snapTurn == null && GorillaTagger.Instance != null)
		{
			this.snapTurn = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
		}
		if (this.snapTurn != null)
		{
			this.snapTurnOverride = true;
			this.snapTurn.SetTurningOverride(this);
		}
	}

	// Token: 0x0600580D RID: 22541 RVA: 0x001CAEA9 File Offset: 0x001C90A9
	private void OnDisable()
	{
		if (this.snapTurnOverride)
		{
			this.snapTurnOverride = false;
			this.snapTurn.UnsetTurningOverride(this);
		}
	}

	// Token: 0x0600580E RID: 22542 RVA: 0x001CAEC6 File Offset: 0x001C90C6
	bool ISnapTurnOverride.TurnOverrideActive()
	{
		return this.snapTurnOverride;
	}

	// Token: 0x0400686D RID: 26733
	private GorillaSnapTurn snapTurn;

	// Token: 0x0400686E RID: 26734
	private bool snapTurnOverride;
}
