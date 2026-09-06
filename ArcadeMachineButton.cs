using System;
using UnityEngine;

// Token: 0x02000424 RID: 1060
public class ArcadeMachineButton : GorillaPressableButton
{
	// Token: 0x14000038 RID: 56
	// (add) Token: 0x0600193B RID: 6459 RVA: 0x0008E71C File Offset: 0x0008C91C
	// (remove) Token: 0x0600193C RID: 6460 RVA: 0x0008E754 File Offset: 0x0008C954
	public event ArcadeMachineButton.ArcadeMachineButtonEvent OnStateChange;

	// Token: 0x0600193D RID: 6461 RVA: 0x0008E789 File Offset: 0x0008C989
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (!this.state)
		{
			this.state = true;
			if (this.OnStateChange != null)
			{
				this.OnStateChange(this.ButtonID, this.state);
			}
		}
	}

	// Token: 0x0600193E RID: 6462 RVA: 0x0008E7C0 File Offset: 0x0008C9C0
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled || !this.state)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.state = false;
		if (this.OnStateChange != null)
		{
			this.OnStateChange(this.ButtonID, this.state);
		}
	}

	// Token: 0x04002458 RID: 9304
	private bool state;

	// Token: 0x04002459 RID: 9305
	[SerializeField]
	private int ButtonID;

	// Token: 0x02000425 RID: 1061
	// (Invoke) Token: 0x06001941 RID: 6465
	public delegate void ArcadeMachineButtonEvent(int id, bool state);
}
