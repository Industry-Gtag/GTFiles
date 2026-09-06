using System;
using UnityEngine;

// Token: 0x0200036C RID: 876
public class RaceConsoleVisual : MonoBehaviour
{
	// Token: 0x0600156B RID: 5483 RVA: 0x00071B28 File Offset: 0x0006FD28
	public void ShowRaceInProgress(int laps)
	{
		this.button1.sharedMaterial = this.inactiveButton;
		this.button3.sharedMaterial = this.inactiveButton;
		this.button5.sharedMaterial = this.inactiveButton;
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		switch (laps)
		{
		default:
			this.button1.sharedMaterial = this.selectedButton;
			this.button1.transform.localPosition = this.buttonPressedOffset;
			return;
		case 3:
			this.button3.sharedMaterial = this.selectedButton;
			this.button3.transform.localPosition = this.buttonPressedOffset;
			return;
		case 5:
			this.button5.sharedMaterial = this.selectedButton;
			this.button5.transform.localPosition = this.buttonPressedOffset;
			return;
		}
	}

	// Token: 0x0600156C RID: 5484 RVA: 0x00071C3C File Offset: 0x0006FE3C
	public void ShowCanStartRace()
	{
		this.button1.transform.localPosition = Vector3.zero;
		this.button3.transform.localPosition = Vector3.zero;
		this.button5.transform.localPosition = Vector3.zero;
		this.button1.sharedMaterial = this.pressableButton;
		this.button3.sharedMaterial = this.pressableButton;
		this.button5.sharedMaterial = this.pressableButton;
	}

	// Token: 0x04001A42 RID: 6722
	[SerializeField]
	private MeshRenderer button1;

	// Token: 0x04001A43 RID: 6723
	[SerializeField]
	private MeshRenderer button3;

	// Token: 0x04001A44 RID: 6724
	[SerializeField]
	private MeshRenderer button5;

	// Token: 0x04001A45 RID: 6725
	[SerializeField]
	private Vector3 buttonPressedOffset;

	// Token: 0x04001A46 RID: 6726
	[SerializeField]
	private Material pressableButton;

	// Token: 0x04001A47 RID: 6727
	[SerializeField]
	private Material selectedButton;

	// Token: 0x04001A48 RID: 6728
	[SerializeField]
	private Material inactiveButton;
}
