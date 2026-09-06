using System;
using UnityEngine;

// Token: 0x02000794 RID: 1940
public class GREnemyBossMoonColliderHelper : MonoBehaviour
{
	// Token: 0x0600314B RID: 12619 RVA: 0x0010BB4E File Offset: 0x00109D4E
	public void Awake()
	{
		if (this.ResizeOnAwake)
		{
			base.transform.localScale = this.ResizeCollider;
		}
	}

	// Token: 0x0600314C RID: 12620 RVA: 0x0010BB6C File Offset: 0x00109D6C
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("GorillaPlayer"))
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component == VRRigCache.Instance.localRig.Rig && Time.time - this.lastTriggered > 0.5f)
			{
				if (this.localPlayer == null)
				{
					this.localPlayer = VRRig.LocalRig.GetComponent<GRPlayer>();
				}
				this.lastTriggered = Time.time;
				this.boss.HitPlayer(this.localPlayer, true);
				this.boss.ShockPlayer();
			}
		}
	}

	// Token: 0x04003F39 RID: 16185
	public bool ResizeOnAwake = true;

	// Token: 0x04003F3A RID: 16186
	public Vector3 ResizeCollider = new Vector3(1.025f, 1.025f, 1.025f);

	// Token: 0x04003F3B RID: 16187
	[SerializeField]
	private GREnemyBossMoon boss;

	// Token: 0x04003F3C RID: 16188
	[SerializeField]
	private GRPlayer localPlayer;

	// Token: 0x04003F3D RID: 16189
	private float lastTriggered;
}
