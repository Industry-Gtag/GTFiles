using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000DAC RID: 3500
public class HideInQuest1AtRuntime : MonoBehaviour
{
	// Token: 0x060055EC RID: 21996 RVA: 0x001C1D0A File Offset: 0x001BFF0A
	private void OnEnable()
	{
		if (PlayFabAuthenticator.instance != null && "Quest1" == PlayFabAuthenticator.instance.platform.ToString())
		{
			Object.Destroy(base.gameObject);
		}
	}
}
