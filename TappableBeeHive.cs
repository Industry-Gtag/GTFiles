using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000A01 RID: 2561
public class TappableBeeHive : Tappable
{
	// Token: 0x060041C5 RID: 16837 RVA: 0x0015E898 File Offset: 0x0015CA98
	private void Awake()
	{
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			Debug.LogError("TappableBeeHive: Disabling because swarmEmergePoint is null at: " + base.transform.GetPath(), this);
			base.enabled = false;
			return;
		}
		base.GetComponent<SlingshotProjectileHitNotifier>().OnProjectileHit += this.OnSlingshotHit;
	}

	// Token: 0x060041C6 RID: 16838 RVA: 0x0015E8FC File Offset: 0x0015CAFC
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (NetworkSystem.Instance.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x060041C7 RID: 16839 RVA: 0x0015E970 File Offset: 0x0015CB70
	public void OnSlingshotHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.swarmEmergeFromPoint == null || this.swarmEmergeToPoint == null)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient && AngryBeeSwarm.instance.isDormant)
		{
			AngryBeeSwarm.instance.Emerge(this.swarmEmergeFromPoint.transform.position, this.swarmEmergeToPoint.transform.position);
		}
	}

	// Token: 0x0400526A RID: 21098
	[SerializeField]
	private GameObject swarmEmergeFromPoint;

	// Token: 0x0400526B RID: 21099
	[SerializeField]
	private GameObject swarmEmergeToPoint;

	// Token: 0x0400526C RID: 21100
	[SerializeField]
	private GameObject honeycombSurface;

	// Token: 0x0400526D RID: 21101
	[SerializeField]
	private float honeycombDisableDuration;

	// Token: 0x0400526E RID: 21102
	[NonSerialized]
	private TimeSince _timeSinceLastTap;

	// Token: 0x0400526F RID: 21103
	private float reenableHoneycombAtTimestamp;

	// Token: 0x04005270 RID: 21104
	private Coroutine reenableHoneycombCoroutine;
}
