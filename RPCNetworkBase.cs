using System;
using UnityEngine;

// Token: 0x02000D2E RID: 3374
internal abstract class RPCNetworkBase : MonoBehaviour
{
	// Token: 0x0600537A RID: 21370
	public abstract void SetClassTarget(IWrappedSerializable target, GorillaWrappedSerializer netHandler);
}
