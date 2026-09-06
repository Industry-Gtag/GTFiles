using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000091 RID: 145
public class ReleaseCageWhenUpsideDown : MonoBehaviour
{
	// Token: 0x06000396 RID: 918 RVA: 0x00014D9A File Offset: 0x00012F9A
	private void Awake()
	{
		this.cage = base.GetComponentInChildren<CrittersCage>();
	}

	// Token: 0x06000397 RID: 919 RVA: 0x00014DA8 File Offset: 0x00012FA8
	private void Update()
	{
		this.cage.inReleasingPosition = Vector3.Angle(base.transform.up, Vector3.down) < this.releaseCritterThreshold;
	}

	// Token: 0x0400041C RID: 1052
	public CrittersCage cage;

	// Token: 0x0400041D RID: 1053
	[FormerlySerializedAs("dumpThreshold")]
	[FormerlySerializedAs("angle")]
	public float releaseCritterThreshold = 30f;
}
