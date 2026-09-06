using System;
using UnityEngine;

// Token: 0x02000B0C RID: 2828
public class MonoBehaviourStatic<T> : MonoBehaviour where T : MonoBehaviour
{
	// Token: 0x170006DA RID: 1754
	// (get) Token: 0x06004866 RID: 18534 RVA: 0x00185A01 File Offset: 0x00183C01
	public static T Instance
	{
		get
		{
			return MonoBehaviourStatic<T>.gInstance;
		}
	}

	// Token: 0x06004867 RID: 18535 RVA: 0x00185A08 File Offset: 0x00183C08
	protected void Awake()
	{
		if (MonoBehaviourStatic<T>.gInstance && MonoBehaviourStatic<T>.gInstance != this)
		{
			Object.Destroy(this);
		}
		MonoBehaviourStatic<T>.gInstance = this as T;
		this.OnAwake();
	}

	// Token: 0x06004868 RID: 18536 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnAwake()
	{
	}

	// Token: 0x04005AFD RID: 23293
	protected static T gInstance;
}
