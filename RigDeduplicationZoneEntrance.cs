using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004DC RID: 1244
public class RigDeduplicationZoneEntrance : MonoBehaviour
{
	// Token: 0x06001E4F RID: 7759 RVA: 0x000A2700 File Offset: 0x000A0900
	private void OnTriggerEnter(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null || !component.isLocal)
		{
			return;
		}
		component.portalShenanigansBit = this.portalShenanigansBit;
		Debug.Log(string.Format("## DeduplicationEntranceZone {0} TriggerEnter {1}", this, other));
		UnityEvent onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter.Invoke();
	}

	// Token: 0x06001E50 RID: 7760 RVA: 0x000A2754 File Offset: 0x000A0954
	private void OnTriggerExit(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null || !component.isLocal)
		{
			return;
		}
		if (component.IsInDisplacementZone)
		{
			Debug.Log(string.Format("## DeduplicationEntranceZone {0} TriggerExit {1} quietly", this, other));
			return;
		}
		component.portalShenanigansBit = false;
		Debug.Log(string.Format("## DeduplicationEntranceZone {0} TriggerExit {1}", this, other));
		UnityEvent onLeavingZone = this.OnLeavingZone;
		if (onLeavingZone == null)
		{
			return;
		}
		onLeavingZone.Invoke();
	}

	// Token: 0x04002889 RID: 10377
	[Tooltip("Value to stamp on the local player's PortalShenanigans bit when they enter. Players carrying different values can't see each other inside the crossing.")]
	[SerializeField]
	private bool portalShenanigansBit;

	// Token: 0x0400288A RID: 10378
	[Tooltip("Fired whenever the local player enters this entrance.")]
	[SerializeField]
	private UnityEvent OnEnter;

	// Token: 0x0400288B RID: 10379
	[Tooltip("Fired when the local player leaves this entrance without having entered the crossing, i.e. when their PortalShenanigans bit is actually reset.")]
	[SerializeField]
	private UnityEvent OnLeavingZone;
}
