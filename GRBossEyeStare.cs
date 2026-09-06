using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000763 RID: 1891
public class GRBossEyeStare : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002FF1 RID: 12273 RVA: 0x00104C26 File Offset: 0x00102E26
	private void Awake()
	{
		this.boss = base.GetComponentInParent<GREnemyBossMoon>();
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x00104C34 File Offset: 0x00102E34
	private void OnEnable()
	{
		this.lastLocalRot = base.transform.localEulerAngles;
		GorillaSlicerSimpleManager.RegisterSliceable(this);
	}

	// Token: 0x06002FF3 RID: 12275 RVA: 0x000DF97B File Offset: 0x000DDB7B
	private void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this);
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x00104C50 File Offset: 0x00102E50
	public void SliceUpdate()
	{
		if (this.boss.CurrAbility != this.lastAbility)
		{
			this.lastLocalRot = base.transform.localEulerAngles;
		}
		if (this.noUpdateAbilities.Contains(this.boss.CurrAbility))
		{
			this.lastLocalRot = base.transform.localEulerAngles;
			this.lastAbility = this.boss.CurrAbility;
			return;
		}
		if (base.transform.localEulerAngles != this.lastLocalRot)
		{
			this.lastLocalRot = base.transform.localEulerAngles;
			if (!this.noUpdateAbilities.Contains(this.boss.CurrAbility))
			{
				this.noUpdateAbilities.Add(this.boss.CurrAbility);
			}
			this.lastAbility = this.boss.CurrAbility;
			return;
		}
		if (this.closestPlayer == null || Time.time > this.lastCheck + this.checkForClosestPlayerCooldown)
		{
			VRRigCache.Instance.GetActiveRigs(this.rigs);
			float num = float.MaxValue;
			for (int i = 0; i < this.rigs.Count; i++)
			{
				float sqrMagnitude = (base.transform.position - this.rigs[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					this.closestPlayer = this.rigs[i].transform;
				}
			}
			this.lastCheck = Time.time;
		}
		this.lastAbility = this.boss.CurrAbility;
		if (this.closestPlayer == null)
		{
			return;
		}
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(Vector3.up, (this.closestPlayer.position - base.transform.position).normalized) * Quaternion.Euler(this.rotOffset), this.lerpAmount);
		this.lastLocalRot = base.transform.localEulerAngles;
	}

	// Token: 0x04003D71 RID: 15729
	private Vector3 lastLocalRot;

	// Token: 0x04003D72 RID: 15730
	private List<GRAbilityBase> noUpdateAbilities = new List<GRAbilityBase>();

	// Token: 0x04003D73 RID: 15731
	private GREnemyBossMoon boss;

	// Token: 0x04003D74 RID: 15732
	private GRAbilityBase lastAbility;

	// Token: 0x04003D75 RID: 15733
	private float lastCheck;

	// Token: 0x04003D76 RID: 15734
	private float checkForClosestPlayerCooldown = 1f;

	// Token: 0x04003D77 RID: 15735
	private Transform closestPlayer;

	// Token: 0x04003D78 RID: 15736
	private List<VRRig> rigs = new List<VRRig>();

	// Token: 0x04003D79 RID: 15737
	public float lerpAmount = 0.3f;

	// Token: 0x04003D7A RID: 15738
	public Vector3 rotOffset;
}
