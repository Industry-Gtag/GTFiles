using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020005EA RID: 1514
public class GravityOverrideVolume : MonoBehaviour
{
	// Token: 0x060025CB RID: 9675 RVA: 0x000C8AA8 File Offset: 0x000C6CA8
	private void OnEnable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter += this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit += this.OnColliderExitedVolume;
		}
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000C8AE6 File Offset: 0x000C6CE6
	private void OnDisable()
	{
		if (this.triggerEvents != null)
		{
			this.triggerEvents.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
			this.triggerEvents.CompositeTriggerExit -= this.OnColliderExitedVolume;
		}
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000C8B24 File Offset: 0x000C6D24
	private void OnColliderEnteredVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
		}
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000C8B64 File Offset: 0x000C6D64
	private void OnColliderExitedVolume(Collider collider)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null && collider == instance.headCollider)
		{
			instance.UnsetGravityOverride(this);
		}
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000C8B98 File Offset: 0x000C6D98
	public void GravityOverrideFunction(GTPlayer player)
	{
		GravityOverrideVolume.GravityType gravityType = this.gravityType;
		if (gravityType == GravityOverrideVolume.GravityType.Directional)
		{
			Vector3 forward = this.referenceTransform.forward;
			player.AddForce(forward * this.strength, ForceMode.Acceleration);
			return;
		}
		if (gravityType != GravityOverrideVolume.GravityType.Radial)
		{
			return;
		}
		Vector3 normalized = (this.referenceTransform.position - player.headCollider.transform.position).normalized;
		player.AddForce(normalized * this.strength, ForceMode.Acceleration);
	}

	// Token: 0x0400316A RID: 12650
	[SerializeField]
	private GravityOverrideVolume.GravityType gravityType;

	// Token: 0x0400316B RID: 12651
	[SerializeField]
	private float strength = 9.8f;

	// Token: 0x0400316C RID: 12652
	[SerializeField]
	[Tooltip("In Radial: the center point of gravity, In Directional: the forward vector of this transform defines the direction")]
	private Transform referenceTransform;

	// Token: 0x0400316D RID: 12653
	[SerializeField]
	private CompositeTriggerEvents triggerEvents;

	// Token: 0x020005EB RID: 1515
	public enum GravityType
	{
		// Token: 0x0400316F RID: 12655
		Directional,
		// Token: 0x04003170 RID: 12656
		Radial
	}
}
