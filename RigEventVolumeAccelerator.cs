using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000D14 RID: 3348
[RequireComponent(typeof(RigEventVolume))]
public class RigEventVolumeAccelerator : MonoBehaviour
{
	// Token: 0x0600531B RID: 21275 RVA: 0x001B6CAC File Offset: 0x001B4EAC
	private void Awake()
	{
		this.rev = base.GetComponent<RigEventVolume>();
		if (Mathf.Abs(this.multiplier) < 100f)
		{
			if (this.multiplier < 0f)
			{
				this.multiplier = -100f;
				return;
			}
			this.multiplier = 100f;
		}
	}

	// Token: 0x0600531C RID: 21276 RVA: 0x001B6CFB File Offset: 0x001B4EFB
	private void FixedUpdate()
	{
		if (!this.rev.LocalRigPresent)
		{
			return;
		}
		GTPlayer.Instance.AddForce(GTPlayer.Instance.AveragedVelocity * this.multiplier * Time.fixedDeltaTime, ForceMode.Acceleration);
	}

	// Token: 0x040064BC RID: 25788
	private const float min_mult = 100f;

	// Token: 0x040064BD RID: 25789
	private RigEventVolume rev;

	// Token: 0x040064BE RID: 25790
	[SerializeField]
	private float multiplier;
}
