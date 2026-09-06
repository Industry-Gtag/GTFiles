using System;
using UnityEngine;

// Token: 0x020007F3 RID: 2035
public class GRShiftSirenLight : MonoBehaviourTick
{
	// Token: 0x06003402 RID: 13314 RVA: 0x0011DECC File Offset: 0x0011C0CC
	public override void Tick()
	{
		if (this.shiftManager == null)
		{
			this.shiftManager = GhostReactor.instance.shiftManager;
			return;
		}
		if (this.redLight.activeSelf != this.shiftManager.ShiftActive)
		{
			this.redLight.SetActive(this.shiftManager.ShiftActive);
		}
		if (this.greenLight.activeSelf == this.shiftManager.ShiftActive)
		{
			this.greenLight.SetActive(!this.shiftManager.ShiftActive);
		}
		if (this.readyRoomLight != null)
		{
			this.readyRoomLight.intensity = (this.shiftManager.ShiftActive ? this.dimLight : this.brightLight);
		}
		if (this.shiftManager.ShiftActive)
		{
			this.redLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
			return;
		}
		this.greenLightParent.localEulerAngles = new Vector3(0f, Time.time * this.rotationRate, 0f);
	}

	// Token: 0x040043C3 RID: 17347
	public float rotationRate = 1.25f;

	// Token: 0x040043C4 RID: 17348
	public Transform greenLightParent;

	// Token: 0x040043C5 RID: 17349
	public Transform redLightParent;

	// Token: 0x040043C6 RID: 17350
	public GameObject redLight;

	// Token: 0x040043C7 RID: 17351
	public GameObject greenLight;

	// Token: 0x040043C8 RID: 17352
	public GhostReactorShiftManager shiftManager;

	// Token: 0x040043C9 RID: 17353
	public float dimLight;

	// Token: 0x040043CA RID: 17354
	public float brightLight;

	// Token: 0x040043CB RID: 17355
	public Light readyRoomLight;
}
