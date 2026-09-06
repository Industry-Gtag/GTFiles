using System;
using UnityEngine;

// Token: 0x020008A0 RID: 2208
public class GorillaParent : MonoBehaviour
{
	// Token: 0x060039AC RID: 14764 RVA: 0x0013A498 File Offset: 0x00138698
	public void Awake()
	{
		if (GorillaParent.instance == null)
		{
			GorillaParent.instance = this;
			GorillaParent.hasInstance = true;
			return;
		}
		if (GorillaParent.instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
	}

	// Token: 0x060039AD RID: 14765 RVA: 0x0013A4D3 File Offset: 0x001386D3
	protected void OnDestroy()
	{
		if (GorillaParent.instance == this)
		{
			GorillaParent.hasInstance = false;
			GorillaParent.instance = null;
		}
	}

	// Token: 0x060039AE RID: 14766 RVA: 0x0013A4F2 File Offset: 0x001386F2
	public static void ReplicatedClientReady()
	{
		GorillaParent.replicatedClientReady = true;
		Action action = GorillaParent.onReplicatedClientReady;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x060039AF RID: 14767 RVA: 0x0013A509 File Offset: 0x00138709
	public static void OnReplicatedClientReady(Action action)
	{
		if (GorillaParent.replicatedClientReady)
		{
			action();
			return;
		}
		GorillaParent.onReplicatedClientReady = (Action)Delegate.Combine(GorillaParent.onReplicatedClientReady, action);
	}

	// Token: 0x040049C0 RID: 18880
	[OnEnterPlay_SetNull]
	public static volatile GorillaParent instance;

	// Token: 0x040049C1 RID: 18881
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x040049C2 RID: 18882
	private int i;

	// Token: 0x040049C3 RID: 18883
	private static bool replicatedClientReady;

	// Token: 0x040049C4 RID: 18884
	private static Action onReplicatedClientReady;
}
