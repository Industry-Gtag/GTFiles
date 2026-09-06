using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A5 RID: 421
public class MazePlayerCollection : MonoBehaviour
{
	// Token: 0x06000B51 RID: 2897 RVA: 0x0003C818 File Offset: 0x0003AA18
	private void Start()
	{
		NetworkSystem.Instance.OnPlayerLeft += this.OnPlayerLeftRoom;
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0003C83B File Offset: 0x0003AA3B
	private void OnDestroy()
	{
		NetworkSystem.Instance.OnPlayerLeft -= this.OnPlayerLeftRoom;
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0003C860 File Offset: 0x0003AA60
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (!this.containedRigs.Contains(component))
		{
			this.containedRigs.Add(component);
		}
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x0003C8B0 File Offset: 0x0003AAB0
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (this.containedRigs.Contains(component))
		{
			this.containedRigs.Remove(component);
		}
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x0003C904 File Offset: 0x0003AB04
	public void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		this.containedRigs.RemoveAll((VRRig r) => ((r != null) ? r.creator : null) == null || r.creator == otherPlayer);
	}

	// Token: 0x04000D9B RID: 3483
	public List<VRRig> containedRigs = new List<VRRig>();

	// Token: 0x04000D9C RID: 3484
	public List<MonkeyeAI> monkeyeAis = new List<MonkeyeAI>();
}
