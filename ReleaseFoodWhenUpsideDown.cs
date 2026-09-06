using System;
using Critters.Scripts;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class ReleaseFoodWhenUpsideDown : MonoBehaviour
{
	// Token: 0x06000399 RID: 921 RVA: 0x00014DE5 File Offset: 0x00012FE5
	private void Awake()
	{
		this.latch = false;
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00014DF0 File Offset: 0x00012FF0
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (!this.dispenser.heldByPlayer)
		{
			return;
		}
		if (Vector3.Angle(base.transform.up, Vector3.down) < this.angle)
		{
			if (this.latch)
			{
				return;
			}
			this.latch = true;
			if (this.nextSpawnTime > (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)))
			{
				return;
			}
			this.nextSpawnTime = (PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) + (double)this.spawnDelay;
			CrittersActor crittersActor = CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.Food, this.foodSubIndex);
			if (!crittersActor.IsNull())
			{
				CrittersFood crittersFood = (CrittersFood)crittersActor;
				crittersFood.MoveActor(this.spawnPoint.position, this.spawnPoint.rotation, false, true, true);
				crittersFood.SetImpulseVelocity(Vector3.zero, Vector3.zero);
				crittersFood.SpawnData(this.maxFood, this.startingFood, this.startingSize);
				return;
			}
		}
		else
		{
			this.latch = false;
		}
	}

	// Token: 0x0400041E RID: 1054
	public CrittersFoodDispenser dispenser;

	// Token: 0x0400041F RID: 1055
	public float angle = 30f;

	// Token: 0x04000420 RID: 1056
	private bool latch;

	// Token: 0x04000421 RID: 1057
	public Transform spawnPoint;

	// Token: 0x04000422 RID: 1058
	public float maxFood;

	// Token: 0x04000423 RID: 1059
	public float startingFood;

	// Token: 0x04000424 RID: 1060
	public float startingSize;

	// Token: 0x04000425 RID: 1061
	public int foodSubIndex;

	// Token: 0x04000426 RID: 1062
	public float spawnDelay = 0.6f;

	// Token: 0x04000427 RID: 1063
	private double nextSpawnTime;
}
