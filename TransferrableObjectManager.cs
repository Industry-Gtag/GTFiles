using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000557 RID: 1367
[DefaultExecutionOrder(1549)]
public class TransferrableObjectManager : MonoBehaviour
{
	// Token: 0x060022C8 RID: 8904 RVA: 0x000BB04E File Offset: 0x000B924E
	protected void Awake()
	{
		if (TransferrableObjectManager.hasInstance && TransferrableObjectManager.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		TransferrableObjectManager.SetInstance(this);
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x000BB071 File Offset: 0x000B9271
	protected void OnDestroy()
	{
		if (TransferrableObjectManager.instance == this)
		{
			TransferrableObjectManager.hasInstance = false;
			TransferrableObjectManager.instance = null;
		}
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x000BB08C File Offset: 0x000B928C
	protected void LateUpdate()
	{
		for (int i = 0; i < TransferrableObjectManager.transObs.Count; i++)
		{
			TransferrableObjectManager.transObs[i].TriggeredLateUpdate();
		}
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x000BB0BE File Offset: 0x000B92BE
	private static void CreateManager()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		TransferrableObjectManager.SetInstance(new GameObject("TransferrableObjectManager").AddComponent<TransferrableObjectManager>());
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x000BB0DC File Offset: 0x000B92DC
	private static void SetInstance(TransferrableObjectManager manager)
	{
		TransferrableObjectManager.instance = manager;
		TransferrableObjectManager.hasInstance = true;
		if (Application.isPlaying)
		{
			Object.DontDestroyOnLoad(manager);
		}
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x000BB0F7 File Offset: 0x000B92F7
	public static void Register(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (!TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Add(transOb);
		}
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x000BB11D File Offset: 0x000B931D
	public static void Unregister(TransferrableObject transOb)
	{
		if (!TransferrableObjectManager.hasInstance)
		{
			TransferrableObjectManager.CreateManager();
		}
		if (TransferrableObjectManager.transObs.Contains(transOb))
		{
			TransferrableObjectManager.transObs.Remove(transOb);
		}
	}

	// Token: 0x04002DE7 RID: 11751
	public static TransferrableObjectManager instance;

	// Token: 0x04002DE8 RID: 11752
	public static bool hasInstance = false;

	// Token: 0x04002DE9 RID: 11753
	public static readonly List<TransferrableObject> transObs = new List<TransferrableObject>(1024);
}
