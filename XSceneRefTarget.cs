using System;
using UnityEngine;

// Token: 0x020003C8 RID: 968
public class XSceneRefTarget : MonoBehaviour
{
	// Token: 0x0600173F RID: 5951 RVA: 0x00086B4A File Offset: 0x00084D4A
	private void Awake()
	{
		this.Register(false);
	}

	// Token: 0x06001740 RID: 5952 RVA: 0x00086B53 File Offset: 0x00084D53
	private void Reset()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
	}

	// Token: 0x06001741 RID: 5953 RVA: 0x00086B60 File Offset: 0x00084D60
	private void OnValidate()
	{
		if (!Application.isPlaying)
		{
			this.Register(false);
		}
	}

	// Token: 0x06001742 RID: 5954 RVA: 0x00086B70 File Offset: 0x00084D70
	public void Register(bool force = false)
	{
		if (this.UniqueID == this.lastRegisteredID && !force)
		{
			return;
		}
		if (this.lastRegisteredID != -1)
		{
			XSceneRefGlobalHub.Unregister(this.lastRegisteredID, this);
		}
		XSceneRefGlobalHub.Register(this.UniqueID, this);
		this.lastRegisteredID = this.UniqueID;
	}

	// Token: 0x06001743 RID: 5955 RVA: 0x00086BBC File Offset: 0x00084DBC
	private void OnDestroy()
	{
		XSceneRefGlobalHub.Unregister(this.UniqueID, this);
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x00086BCA File Offset: 0x00084DCA
	private void AssignNewID()
	{
		this.UniqueID = XSceneRefTarget.CreateNewID();
		this.Register(false);
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x00086BE0 File Offset: 0x00084DE0
	public static int CreateNewID()
	{
		int num = (int)((DateTime.Now - XSceneRefTarget.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
		if (num <= XSceneRefTarget.lastAssignedID)
		{
			XSceneRefTarget.lastAssignedID++;
			return XSceneRefTarget.lastAssignedID;
		}
		XSceneRefTarget.lastAssignedID = num;
		return num;
	}

	// Token: 0x0400227C RID: 8828
	public int UniqueID;

	// Token: 0x0400227D RID: 8829
	[NonSerialized]
	private int lastRegisteredID = -1;

	// Token: 0x0400227E RID: 8830
	private static DateTime epoch = new DateTime(2024, 1, 1);

	// Token: 0x0400227F RID: 8831
	private static int lastAssignedID;
}
