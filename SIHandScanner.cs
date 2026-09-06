using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000143 RID: 323
public class SIHandScanner : MonoBehaviour
{
	// Token: 0x06000817 RID: 2071 RVA: 0x0002C6D3 File Offset: 0x0002A8D3
	public void HandScanned(SIPlayer scannedPlayer)
	{
		if (!scannedPlayer.gamePlayer.IsLocal())
		{
			return;
		}
		this.onHandScanned.Invoke(NetworkSystem.Instance.LocalPlayerID);
	}

	// Token: 0x04000A3A RID: 2618
	public UnityEvent<int> onHandScanned;
}
