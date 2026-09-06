using System;
using System.Collections;
using GorillaLocomotion;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000E37 RID: 3639
public class TeleportNode : GorillaTriggerBox
{
	// Token: 0x060058F4 RID: 22772 RVA: 0x001CEC48 File Offset: 0x001CCE48
	public void SetDestinationOverride(Transform destination)
	{
		this.destinationOverride = destination;
	}

	// Token: 0x060058F5 RID: 22773 RVA: 0x001CEC51 File Offset: 0x001CCE51
	public void ClearDestinationOverride()
	{
		this.destinationOverride = null;
	}

	// Token: 0x060058F6 RID: 22774 RVA: 0x001CEC5C File Offset: 0x001CCE5C
	public override void OnBoxTriggered()
	{
		if (this.subsOnly && !SubscriptionManager.IsLocalSubscribed())
		{
			return;
		}
		if (Time.time - this.teleportTime < 0.1f)
		{
			return;
		}
		base.OnBoxTriggered();
		Transform transform;
		if (!this.teleportFromRef.TryResolve<Transform>(out transform))
		{
			Debug.LogError("[TeleportNode] Failed to resolve teleportFromRef.");
			return;
		}
		Transform transform2;
		if (this.destinationOverride != null)
		{
			transform2 = this.destinationOverride;
		}
		else if (!this.teleportToRef.TryResolve<Transform>(out transform2))
		{
			Debug.LogError("[TeleportNode] Failed to resolve teleportToRef.");
			return;
		}
		Debug.LogWarning(string.Concat(new string[]
		{
			"[TeleportNode] '",
			base.gameObject.name,
			"' fired -> destination '",
			transform2.name,
			"' ",
			string.Format("(override={0})", this.destinationOverride != null)
		}));
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			Debug.LogError("[TeleportNode] GTPlayer.Instance is null.");
			return;
		}
		Physics.SyncTransforms();
		Vector3 vector = transform2.transform.position;
		if (this.seamless)
		{
			vector = transform2.TransformPoint(transform.InverseTransformPoint(instance.transform.position));
		}
		Quaternion quaternion = Quaternion.Inverse(transform.rotation) * instance.transform.rotation;
		Quaternion quaternion2 = transform2.rotation * quaternion;
		base.StartCoroutine(this.DelayedTeleport(instance, vector, quaternion2));
		this.teleportTime = Time.time;
	}

	// Token: 0x060058F7 RID: 22775 RVA: 0x001CEDCE File Offset: 0x001CCFCE
	private IEnumerator DelayedTeleport(GTPlayer p, Vector3 position, Quaternion rotation)
	{
		yield return null;
		p.TeleportTo(position, rotation, this.keepVelocity, !this.seamless);
		if (this.teleportToZone != GTZone.none)
		{
			ZoneManagement.SetActiveZone(this.teleportToZone);
		}
		this.onTeleport.Invoke();
		yield break;
	}

	// Token: 0x04006924 RID: 26916
	[SerializeField]
	private XSceneRef teleportFromRef;

	// Token: 0x04006925 RID: 26917
	[SerializeField]
	private XSceneRef teleportToRef;

	// Token: 0x04006926 RID: 26918
	[SerializeField]
	private GTZone teleportToZone = GTZone.none;

	// Token: 0x04006927 RID: 26919
	[SerializeField]
	private bool seamless = true;

	// Token: 0x04006928 RID: 26920
	[SerializeField]
	private bool keepVelocity = true;

	// Token: 0x04006929 RID: 26921
	[SerializeField]
	private bool subsOnly;

	// Token: 0x0400692A RID: 26922
	[SerializeField]
	private UnityEvent onTeleport;

	// Token: 0x0400692B RID: 26923
	private float teleportTime;

	// Token: 0x0400692C RID: 26924
	private Transform destinationOverride;
}
