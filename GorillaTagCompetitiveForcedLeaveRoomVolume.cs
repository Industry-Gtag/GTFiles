using System;
using GorillaGameModes;
using UnityEngine;

// Token: 0x020008A8 RID: 2216
public class GorillaTagCompetitiveForcedLeaveRoomVolume : MonoBehaviour
{
	// Token: 0x06003A01 RID: 14849 RVA: 0x0013C250 File Offset: 0x0013A450
	private void Start()
	{
		this.VolumeCollider = base.GetComponent<Collider>();
		this.CompetitiveManager = GameMode.GetGameModeInstance(GameModeType.InfectionCompetitive) as GorillaTagCompetitiveManager;
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.RegisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x06003A02 RID: 14850 RVA: 0x0013C28A File Offset: 0x0013A48A
	private void OnDestroy()
	{
		if (this.CompetitiveManager != null)
		{
			this.CompetitiveManager.UnregisterForcedLeaveVolume(this);
		}
	}

	// Token: 0x06003A03 RID: 14851 RVA: 0x0013C2A8 File Offset: 0x0013A4A8
	public bool ContainsPoint(Vector3 position)
	{
		SphereCollider sphereCollider = this.VolumeCollider as SphereCollider;
		if (sphereCollider != null)
		{
			return Vector3.SqrMagnitude(position - (sphereCollider.transform.position + sphereCollider.center)) <= sphereCollider.radius * sphereCollider.radius;
		}
		BoxCollider boxCollider = this.VolumeCollider as BoxCollider;
		return boxCollider != null && boxCollider.bounds.Contains(position);
	}

	// Token: 0x04004A20 RID: 18976
	private GorillaTagCompetitiveManager CompetitiveManager;

	// Token: 0x04004A21 RID: 18977
	private Collider VolumeCollider;
}
