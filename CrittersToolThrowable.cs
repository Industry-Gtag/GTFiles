using System;
using System.Diagnostics;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class CrittersToolThrowable : CrittersActor
{
	// Token: 0x06000309 RID: 777 RVA: 0x00011EDA File Offset: 0x000100DA
	public override void Initialize()
	{
		base.Initialize();
		this.hasBeenGrabbedByPlayer = false;
		this.shouldDisable = false;
		this.hasTriggeredSinceLastGrab = false;
		this._sqrActivationSpeed = this.requiredActivationSpeed * this.requiredActivationSpeed;
	}

	// Token: 0x0600030A RID: 778 RVA: 0x00011F0A File Offset: 0x0001010A
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.hasBeenGrabbedByPlayer = true;
		this.hasTriggeredSinceLastGrab = false;
		this.OnPickedUp();
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00011F30 File Offset: 0x00010130
	public void OnCollisionEnter(Collision collision)
	{
		if (CrittersManager.instance.containerLayer.Contains(collision.gameObject.layer))
		{
			return;
		}
		if (this.requiresPlayerGrabBeforeActivate && !this.hasBeenGrabbedByPlayer)
		{
			return;
		}
		if (this._sqrActivationSpeed > 0f && collision.relativeVelocity.sqrMagnitude < this._sqrActivationSpeed)
		{
			return;
		}
		if (this.onlyTriggerOncePerGrab && this.hasTriggeredSinceLastGrab)
		{
			return;
		}
		if (this.onlyTriggerOnDirectCritterHit)
		{
			CrittersPawn component = collision.gameObject.GetComponent<CrittersPawn>();
			if (component != null && component.isActiveAndEnabled)
			{
				this.hasTriggeredSinceLastGrab = true;
				this.OnImpactCritter(component);
			}
		}
		else
		{
			Vector3 point = collision.contacts[0].point;
			Vector3 normal = collision.contacts[0].normal;
			this.hasTriggeredSinceLastGrab = true;
			this.OnImpact(point, normal);
		}
		if (this.destroyOnImpact)
		{
			this.shouldDisable = true;
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnImpactCritter(CrittersPawn impactedCritter)
	{
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnPickedUp()
	{
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0001201C File Offset: 0x0001021C
	[Conditional("DRAW_DEBUG")]
	protected void ShowDebugVisualization(Vector3 position, float scale, float duration = 0f)
	{
		if (!this.debugImpactPrefab)
		{
			return;
		}
		DelayedDestroyObject delayedDestroyObject = Object.Instantiate<DelayedDestroyObject>(this.debugImpactPrefab, position, Quaternion.identity);
		delayedDestroyObject.transform.localScale *= scale;
		if (duration != 0f)
		{
			delayedDestroyObject.lifetime = duration;
		}
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00012070 File Offset: 0x00010270
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (this.shouldDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return flag;
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0001209C File Offset: 0x0001029C
	public override void TogglePhysics(bool enable)
	{
		if (enable)
		{
			this.rb.isKinematic = false;
			this.rb.interpolation = RigidbodyInterpolation.Interpolate;
			this.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
			return;
		}
		this.rb.isKinematic = true;
		this.rb.interpolation = RigidbodyInterpolation.None;
		this.rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}

	// Token: 0x0400036A RID: 874
	[Header("Throwable")]
	public bool requiresPlayerGrabBeforeActivate = true;

	// Token: 0x0400036B RID: 875
	public float requiredActivationSpeed = 2f;

	// Token: 0x0400036C RID: 876
	public bool onlyTriggerOnDirectCritterHit;

	// Token: 0x0400036D RID: 877
	public bool destroyOnImpact = true;

	// Token: 0x0400036E RID: 878
	public bool onlyTriggerOncePerGrab = true;

	// Token: 0x0400036F RID: 879
	[Header("Debug")]
	[SerializeField]
	private DelayedDestroyObject debugImpactPrefab;

	// Token: 0x04000370 RID: 880
	private bool hasBeenGrabbedByPlayer;

	// Token: 0x04000371 RID: 881
	protected bool shouldDisable;

	// Token: 0x04000372 RID: 882
	private bool hasTriggeredSinceLastGrab;

	// Token: 0x04000373 RID: 883
	private float _sqrActivationSpeed;
}
