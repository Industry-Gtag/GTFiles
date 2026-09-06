using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020007ED RID: 2029
[Serializable]
public class GRSenseNearby
{
	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x060033D6 RID: 13270 RVA: 0x0011CACD File Offset: 0x0011ACCD
	private bool BossEntityPresent
	{
		get
		{
			return GhostReactorManager.Get(this._entity).GetBossEntity() != null;
		}
	}

	// Token: 0x060033D7 RID: 13271 RVA: 0x0011CAE5 File Offset: 0x0011ACE5
	public void Setup(Transform headTransform, GameEntity entity)
	{
		this.rigsNearby = new List<VRRig>();
		this.headTransform = headTransform;
		this._entity = entity;
	}

	// Token: 0x060033D8 RID: 13272 RVA: 0x0011CB00 File Offset: 0x0011AD00
	public void OnHitByPlayer(int hitByActorId)
	{
		GRPlayer grplayer = GRPlayer.Get(hitByActorId);
		if (grplayer != null)
		{
			VRRig rig = grplayer.gamePlayer.rig;
			if (!this.rigsNearby.Contains(rig))
			{
				this.rigsNearby.Add(rig);
			}
		}
	}

	// Token: 0x060033D9 RID: 13273 RVA: 0x0011CB44 File Offset: 0x0011AD44
	public void UpdateNearby(List<VRRig> allRigs, GRSenseLineOfSight senseLineOfSight)
	{
		Vector3 position = this.headTransform.position;
		Vector3 vector = this.headTransform.rotation * Vector3.forward;
		this.RemoveNotNearby(position);
		this.AddNearby(position, vector, allRigs);
		this.RemoveNoLineOfSight(position, senseLineOfSight);
	}

	// Token: 0x060033DA RID: 13274 RVA: 0x0011CB8B File Offset: 0x0011AD8B
	public bool IsAnyoneNearby()
	{
		return !GhostReactorManager.AggroDisabled && this.rigsNearby != null && this.rigsNearby.Count > 0;
	}

	// Token: 0x060033DB RID: 13275 RVA: 0x0011CBAC File Offset: 0x0011ADAC
	public bool IsAnyoneNearby(float range, bool ignoreBossEntity = false)
	{
		if (!ignoreBossEntity && this.BossEntityPresent && this.rigsNearby.Count > 0)
		{
			return true;
		}
		if (!this.IsAnyoneNearby())
		{
			return false;
		}
		Vector3 position = this.headTransform.position;
		float num = range * range;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			if (!(this.rigsNearby[i] == null) && (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude <= num)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060033DC RID: 13276 RVA: 0x0011CC3D File Offset: 0x0011AE3D
	public static Vector3 GetRigTestLocation(VRRig rig)
	{
		return rig.transform.position;
	}

	// Token: 0x060033DD RID: 13277 RVA: 0x0011CC4C File Offset: 0x0011AE4C
	public void AddNearby(Vector3 position, Vector3 forward, List<VRRig> allRigs)
	{
		if (this.BossEntityPresent)
		{
			foreach (VRRig vrrig in allRigs)
			{
				if (!this.rigsNearby.Contains(vrrig))
				{
					this.rigsNearby.Add(vrrig);
				}
			}
			return;
		}
		float num = this.range * this.range;
		float num2 = Mathf.Cos(this.fov * 0.017453292f);
		for (int i = 0; i < allRigs.Count; i++)
		{
			VRRig vrrig2 = allRigs[i];
			GRPlayer component = vrrig2.GetComponent<GRPlayer>();
			if (component.State != GRPlayer.GRPlayerState.Ghost && !component.InStealthMode && !this.rigsNearby.Contains(vrrig2))
			{
				Vector3 vector = GRSenseNearby.GetRigTestLocation(vrrig2) - position;
				float sqrMagnitude = vector.sqrMagnitude;
				float num3 = this.hearingRange * this.hearingRange;
				if (sqrMagnitude >= num3)
				{
					if (sqrMagnitude >= num)
					{
						goto IL_0116;
					}
					if (sqrMagnitude > 0f)
					{
						float num4 = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(vector / num4, forward) < num2)
						{
							goto IL_0116;
						}
					}
				}
				this.rigsNearby.Add(vrrig2);
			}
			IL_0116:;
		}
	}

	// Token: 0x060033DE RID: 13278 RVA: 0x0011CD94 File Offset: 0x0011AF94
	public void RemoveNotNearby(Vector3 position)
	{
		if (this.BossEntityPresent)
		{
			return;
		}
		float num = this.exitRange * this.exitRange;
		int i = 0;
		while (i < this.rigsNearby.Count)
		{
			VRRig vrrig = this.rigsNearby[i];
			if (!(vrrig != null))
			{
				goto IL_0061;
			}
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if ((GRSenseNearby.GetRigTestLocation(vrrig) - position).sqrMagnitude > num || component.State == GRPlayer.GRPlayerState.Ghost || component.InStealthMode)
			{
				goto IL_0061;
			}
			IL_0071:
			i++;
			continue;
			IL_0061:
			this.rigsNearby.RemoveAt(i);
			i--;
			goto IL_0071;
		}
	}

	// Token: 0x060033DF RID: 13279 RVA: 0x0011CE24 File Offset: 0x0011B024
	public void RemoveNoLineOfSight(Vector3 headPos, GRSenseLineOfSight senseLineOfSight)
	{
		if (this.BossEntityPresent)
		{
			return;
		}
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			Vector3 rigTestLocation = GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]);
			if (!senseLineOfSight.HasLineOfSight(headPos, rigTestLocation))
			{
				this.rigsNearby.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x060033E0 RID: 13280 RVA: 0x0011CE7C File Offset: 0x0011B07C
	public VRRig PickClosest(out float outDistanceSq)
	{
		Vector3 position = this.headTransform.position;
		float num = float.MaxValue;
		VRRig vrrig = null;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			float sqrMagnitude = (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				vrrig = this.rigsNearby[i];
			}
		}
		outDistanceSq = num;
		return vrrig;
	}

	// Token: 0x04004385 RID: 17285
	public float range;

	// Token: 0x04004386 RID: 17286
	public float hearingRange;

	// Token: 0x04004387 RID: 17287
	public float exitRange;

	// Token: 0x04004388 RID: 17288
	public float fov;

	// Token: 0x04004389 RID: 17289
	[ReadOnly]
	public List<VRRig> rigsNearby;

	// Token: 0x0400438A RID: 17290
	private Transform headTransform;

	// Token: 0x0400438B RID: 17291
	private GameEntity _entity;
}
