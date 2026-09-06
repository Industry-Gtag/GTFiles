using System;
using UnityEngine;

// Token: 0x020004DE RID: 1246
public class RigDuplicationZone : RigDisplacementZone
{
	// Token: 0x14000041 RID: 65
	// (add) Token: 0x06001E58 RID: 7768 RVA: 0x000A2874 File Offset: 0x000A0A74
	// (remove) Token: 0x06001E59 RID: 7769 RVA: 0x000A28A8 File Offset: 0x000A0AA8
	public static event RigDuplicationZone.RigDuplicationZoneAction OnEnabled;

	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001E5A RID: 7770 RVA: 0x000A28DB File Offset: 0x000A0ADB
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x06001E5B RID: 7771 RVA: 0x000A28E3 File Offset: 0x000A0AE3
	private void OnEnable()
	{
		RigDuplicationZone.OnEnabled += this.RigDuplicationZone_OnEnabled;
		if (RigDuplicationZone.OnEnabled != null)
		{
			RigDuplicationZone.OnEnabled(this);
		}
	}

	// Token: 0x06001E5C RID: 7772 RVA: 0x000A2908 File Offset: 0x000A0B08
	protected override void OnDisable()
	{
		base.OnDisable();
		RigDuplicationZone.OnEnabled -= this.RigDuplicationZone_OnEnabled;
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x000A2921 File Offset: 0x000A0B21
	private void RigDuplicationZone_OnEnabled(RigDuplicationZone z)
	{
		if (z == this)
		{
			return;
		}
		if (z.id != this.id)
		{
			return;
		}
		this.SetOtherZone(z);
		z.SetOtherZone(this);
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000A294F File Offset: 0x000A0B4F
	private void SetOtherZone(RigDuplicationZone z)
	{
		this.otherZone = z;
		if (this.seeSwapFromZone == null)
		{
			this.seeSwapFromZone = this.otherZone;
		}
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x000A2974 File Offset: 0x000A0B74
	public override Vector3 GetDisplacementForRig(VRRig rig, Vector3 undisplacedPosition)
	{
		if (this.seeSwapFromZone == null)
		{
			Debug.LogError("RigDuplicationZone doesn't have an other zone!", base.gameObject);
			return Vector3.zero;
		}
		if (this.seeSwapFromZone.localPlayerInZone)
		{
			return this.seeSwapFromZone.transform.TransformPoint(base.transform.InverseTransformPoint(undisplacedPosition)) - undisplacedPosition;
		}
		return Vector3.zero;
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x000A29DA File Offset: 0x000A0BDA
	public override bool IsDisplacingRig(VRRig rig)
	{
		return this.otherZone.localPlayerInZone;
	}

	// Token: 0x0400288E RID: 10382
	private RigDuplicationZone otherZone;

	// Token: 0x0400288F RID: 10383
	[SerializeField]
	private string id;

	// Token: 0x04002890 RID: 10384
	[Tooltip("Leave blank for a regular duplication zone. For a portal effect, set this to the zone from which players looking at this zone should see its contents swapped")]
	[SerializeField]
	private RigDuplicationZone seeSwapFromZone;

	// Token: 0x020004DF RID: 1247
	// (Invoke) Token: 0x06001E63 RID: 7779
	public delegate void RigDuplicationZoneAction(RigDuplicationZone z);
}
