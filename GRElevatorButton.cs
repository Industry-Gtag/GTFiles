using System;
using UnityEngine;

// Token: 0x02000779 RID: 1913
public class GRElevatorButton : MonoBehaviour
{
	// Token: 0x06003084 RID: 12420 RVA: 0x00106DD8 File Offset: 0x00104FD8
	private void Awake()
	{
		if (this.disableDelayed == null)
		{
			this.disableDelayed = this.buttonLit.GetComponent<DisableGameObjectDelayed>();
		}
		if (this.tempLight)
		{
			this.disableDelayed.enabled = false;
			return;
		}
		this.disableDelayed.delayTime = this.litUpTime;
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x00106E2A File Offset: 0x0010502A
	public void Pressed()
	{
		this.buttonLit.SetActive(true);
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x00106E38 File Offset: 0x00105038
	public void Depressed()
	{
		this.buttonLit.SetActive(false);
	}

	// Token: 0x04003E26 RID: 15910
	public GRElevator.ButtonType buttonType;

	// Token: 0x04003E27 RID: 15911
	public GameObject buttonLit;

	// Token: 0x04003E28 RID: 15912
	public float litUpTime;

	// Token: 0x04003E29 RID: 15913
	public DisableGameObjectDelayed disableDelayed;

	// Token: 0x04003E2A RID: 15914
	public bool tempLight;
}
