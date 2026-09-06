using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020009F6 RID: 2550
public class StageMicrophone : MonoBehaviour
{
	// Token: 0x0600416C RID: 16748 RVA: 0x0015C33B File Offset: 0x0015A53B
	private void Awake()
	{
		StageMicrophone.Instance = this;
	}

	// Token: 0x0600416D RID: 16749 RVA: 0x0015C343 File Offset: 0x0015A543
	public bool IsPlayerAmplified(VRRig player)
	{
		return (player.GetMouthPosition() - base.transform.position).IsShorterThan(this.PickupRadius);
	}

	// Token: 0x0600416E RID: 16750 RVA: 0x0015C366 File Offset: 0x0015A566
	public float GetPlayerSpatialBlend(VRRig player)
	{
		if (!this.IsPlayerAmplified(player))
		{
			return 0.9f;
		}
		return this.AmplifiedSpatialBlend;
	}

	// Token: 0x04005219 RID: 21017
	public static StageMicrophone Instance;

	// Token: 0x0400521A RID: 21018
	[SerializeField]
	private float PickupRadius;

	// Token: 0x0400521B RID: 21019
	[SerializeField]
	private float AmplifiedSpatialBlend;
}
