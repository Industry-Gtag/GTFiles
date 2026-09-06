using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002E7 RID: 743
public class LeafBlowerEffects : MonoBehaviour, ISpawnable
{
	// Token: 0x170001DE RID: 478
	// (get) Token: 0x060012E4 RID: 4836 RVA: 0x00064A25 File Offset: 0x00062C25
	// (set) Token: 0x060012E5 RID: 4837 RVA: 0x00064A2D File Offset: 0x00062C2D
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001DF RID: 479
	// (get) Token: 0x060012E6 RID: 4838 RVA: 0x00064A36 File Offset: 0x00062C36
	// (set) Token: 0x060012E7 RID: 4839 RVA: 0x00064A3E File Offset: 0x00062C3E
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060012E8 RID: 4840 RVA: 0x00002C2D File Offset: 0x00000E2D
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060012E9 RID: 4841 RVA: 0x00064A48 File Offset: 0x00062C48
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.headToleranceAngleCos = Mathf.Cos(0.017453292f * this.headToleranceAngle);
		this.squareHitAngleCos = Mathf.Cos(0.017453292f * this.squareHitAngle);
		this.fan = rig.cosmeticReferences.Get(this.fanRef).GetComponent<CosmeticFan>();
	}

	// Token: 0x060012EA RID: 4842 RVA: 0x00064A9F File Offset: 0x00062C9F
	public void StartFan()
	{
		this.fan.Run();
	}

	// Token: 0x060012EB RID: 4843 RVA: 0x00064AAC File Offset: 0x00062CAC
	public void StopFan()
	{
		this.fan.Stop();
	}

	// Token: 0x060012EC RID: 4844 RVA: 0x00064AB9 File Offset: 0x00062CB9
	public void UpdateEffects()
	{
		this.ProjectParticles();
		this.BlowFaces();
	}

	// Token: 0x060012ED RID: 4845 RVA: 0x00064AC8 File Offset: 0x00062CC8
	public void ProjectParticles()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(this.gunBarrel.transform.position, this.gunBarrel.transform.forward, out raycastHit, this.projectionRange, this.raycastLayers))
		{
			SpawnOnEnter component = raycastHit.collider.GetComponent<SpawnOnEnter>();
			if (component != null)
			{
				component.OnTriggerEnter(raycastHit.collider);
			}
			if (Vector3.Dot(raycastHit.normal, this.gunBarrel.transform.forward) < -this.squareHitAngleCos)
			{
				this.squareHitParticleSystem.transform.position = raycastHit.point;
				this.squareHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Play(true);
					return;
				}
			}
			else
			{
				this.angledHitParticleSystem.transform.position = raycastHit.point;
				this.angledHitParticleSystem.transform.rotation = Quaternion.LookRotation(raycastHit.normal, this.gunBarrel.transform.forward);
				if (this.angledHitParticleSystem != this.squareHitParticleSystem && this.squareHitParticleSystem.isPlaying)
				{
					this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
				}
				if (!this.angledHitParticleSystem.isPlaying)
				{
					this.angledHitParticleSystem.Play(true);
					return;
				}
			}
		}
		else
		{
			this.StopEffects();
		}
	}

	// Token: 0x060012EE RID: 4846 RVA: 0x00064C7A File Offset: 0x00062E7A
	public void StopEffects()
	{
		this.angledHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		this.squareHitParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

	// Token: 0x060012EF RID: 4847 RVA: 0x00064C98 File Offset: 0x00062E98
	public void BlowFaces()
	{
		Vector3 position = this.gunBarrel.transform.position;
		Vector3 forward = this.gunBarrel.transform.forward;
		if (NetworkSystem.Instance.InRoom)
		{
			using (IEnumerator<RigContainer> enumerator = VRRigCache.ActiveRigContainers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RigContainer rigContainer = enumerator.Current;
					this.TryBlowFace(rigContainer.Rig, position, forward);
				}
				return;
			}
		}
		this.TryBlowFace(VRRig.LocalRig, position, forward);
	}

	// Token: 0x060012F0 RID: 4848 RVA: 0x00064D28 File Offset: 0x00062F28
	private void TryBlowFace(VRRig rig, Vector3 origin, Vector3 directionNormalized)
	{
		Transform rigTarget = rig.head.rigTarget;
		Vector3 vector = rigTarget.position - origin;
		float num = Vector3.Dot(vector, directionNormalized);
		if (num < 0f || num > this.projectionRange)
		{
			return;
		}
		if ((vector - num * directionNormalized).IsLongerThan(this.projectionWidth))
		{
			return;
		}
		if (Vector3.Dot(-rigTarget.forward, vector.normalized) < this.headToleranceAngleCos)
		{
			return;
		}
		rig.GetComponent<GorillaMouthFlap>().EnableLeafBlower();
	}

	// Token: 0x04001717 RID: 5911
	[SerializeField]
	private GameObject gunBarrel;

	// Token: 0x04001718 RID: 5912
	[SerializeField]
	private float projectionRange;

	// Token: 0x04001719 RID: 5913
	[SerializeField]
	private float projectionWidth;

	// Token: 0x0400171A RID: 5914
	[SerializeField]
	private float headToleranceAngle;

	// Token: 0x0400171B RID: 5915
	[SerializeField]
	private LayerMask raycastLayers;

	// Token: 0x0400171C RID: 5916
	[SerializeField]
	private ParticleSystem angledHitParticleSystem;

	// Token: 0x0400171D RID: 5917
	[SerializeField]
	private ParticleSystem squareHitParticleSystem;

	// Token: 0x0400171E RID: 5918
	[SerializeField]
	private float squareHitAngle;

	// Token: 0x0400171F RID: 5919
	[SerializeField]
	private CosmeticRefID fanRef;

	// Token: 0x04001720 RID: 5920
	private float headToleranceAngleCos;

	// Token: 0x04001721 RID: 5921
	private float squareHitAngleCos;

	// Token: 0x04001722 RID: 5922
	private CosmeticFan fan;
}
