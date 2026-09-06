using System;
using UnityEngine;

// Token: 0x020004DD RID: 1245
public abstract class RigDisplacementZone : MonoBehaviour
{
	// Token: 0x06001E52 RID: 7762 RVA: 0x000A27BC File Offset: 0x000A09BC
	protected virtual void OnTriggerEnter(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		component.SetDisplacementZone(this);
		if (component.isLocal)
		{
			Debug.Log(string.Format("## Enter Displacement Zone {0} {1}", this, other));
			this.localPlayerInZone = true;
		}
	}

	// Token: 0x06001E53 RID: 7763 RVA: 0x000A2804 File Offset: 0x000A0A04
	protected virtual void OnTriggerExit(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		component.ClearDisplacementZone(this);
		if (component.isLocal)
		{
			Debug.Log(string.Format("## Exit Displacement Zone {0} {1}", this, other));
			this.localPlayerInZone = false;
		}
	}

	// Token: 0x06001E54 RID: 7764 RVA: 0x000A2849 File Offset: 0x000A0A49
	protected virtual void OnDisable()
	{
		if (!this.localPlayerInZone)
		{
			return;
		}
		this.localPlayerInZone = false;
		if (VRRig.LocalRig != null)
		{
			VRRig.LocalRig.ClearDisplacementZone(this);
		}
	}

	// Token: 0x06001E55 RID: 7765
	public abstract Vector3 GetDisplacementForRig(VRRig rig, Vector3 undisplacedPosition);

	// Token: 0x06001E56 RID: 7766
	public abstract bool IsDisplacingRig(VRRig rig);

	// Token: 0x0400288C RID: 10380
	protected bool localPlayerInZone;
}
